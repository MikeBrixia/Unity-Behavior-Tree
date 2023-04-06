using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using BT.Runtime;
using BT.Editor;
using Editor.BehaviorTree.BT_Elements;

namespace BT
{
    ///<summary>
    /// The behavior tree graph view in which the user is going to
    /// Create, move and delete behavior tree nodes
    ///</summary>
    public class BehaviorTreeGraphView : GraphView
    {

        public new class UxmlFactory : UxmlFactory<BehaviorTreeGraphView, UxmlTraits> { }
        
        ///<summary>
        /// The tree the graph is currently focusing on
        ///</summary>
        public BehaviorTree tree;

        ///<summary>
        /// the position of the mouse in the graph
        ///</summary>
        private Vector2 mousePosition;
        
        ///<summary>
        /// Called when the user select a node inside the graph
        ///</summary>
        public Action<BT_NodeView> onNodeSelected;
        
        /// <summary>
        /// Called when the user selects a child node inside the graph.
        /// </summary>
        public Action<BT_ChildNodeView> onChildNodeSelected;
        
        ///<summary>
        /// Called when the user select a node visual element inside the graph.
        /// Node visual element are all the nodes which can be attached to other nodes.
        ///</summary>
        public Action<BT_ChildNodeView> onNodeVisualElementSelected;
        
        ///<summary>
        /// Called when the user press a mouse button while it's cursor is inside
        /// the graph window.
        ///</summary>
        private EventCallback<MouseDownEvent> mousePressedEvent;

        ///<summary>
        /// all the data which we want to copy with CTRL-C
        ///</summary>
        private List<BT_NodeView> copyCache = new List<BT_NodeView>();

        public BehaviorTreeGraphView()
        {
            // Insert background under everything else
            Insert(0, new GridBackground());

            // Load style sheet
            var StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ai.behavior-tree/Editor/BehaviorTree/GridBackgroundStyle.uss");
            styleSheets.Add(StyleSheet);

            // Add manipulators to this graph
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Setup custom undo/redo events
            Undo.undoRedoPerformed += OnUndoRedo;

            // Register mouse callbacks
            mousePressedEvent = OnGraphSelected;
            RegisterCallback<MouseDownEvent>(mousePressedEvent, TrickleDown.TrickleDown);

            // Keyboard callbacks
            EventCallback<KeyDownEvent> keyboardPressedEvent = OnKeyboardPressed;
            RegisterCallback<KeyDownEvent>(keyboardPressedEvent);

            // Copy/paste callbacks
            serializeGraphElements += OnCopy;
            unserializeAndPaste += OnPaste;
        }

        ///<summary>
        /// Called when the user express the intention to paste some nodes(ctrl-v)
        ///</summary>
        private void OnPaste(string operationName, string data)
        {
            // Paste copied node views 
            foreach (BT_NodeView copiedNode in copyCache)
            {
                BT_Node node = NodeFactory.CreateNode(copiedNode.node.GetType(), tree);
            }
            // Once we finished pasting nodes, clear the copy cache
            // and repopulate the view
            copyCache.Clear();
            PopulateView();
        }

        ///<summary>
        /// Called when the user express the intention to copy some nodes(ctrl-c)
        ///</summary>
        private string OnCopy(IEnumerable<GraphElement> elements)
        {
            copyCache.Clear();
            foreach (GraphElement element in elements)
            {
                BT_NodeView nodeToCopy = element as BT_NodeView;
                if (nodeToCopy != null)
                {
                    copyCache.Add(nodeToCopy);
                }
            }
            return copyCache.ToString();
        }

        ///<summary>
        /// Called each time there is a keyboard event inside the graph.
        ///</summary>
        private void OnKeyboardPressed(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                BT_DecoratorView decoratorView = BehaviorTreeManager.selectedObject as BT_DecoratorView;
                if (decoratorView != null)
                {
                    // Remove decorator view from action node
                    BT_ActionNode actionNode = decoratorView.parentView.node as BT_ActionNode;
                    if (actionNode != null)
                    {
                        Undo.RecordObject(actionNode, "Undo delete decorator");
                        actionNode.decorators.Remove(decoratorView.node as BT_Decorator);
                        EditorUtility.SetDirty(actionNode);
                    }

                    // Remove decorator view from composite node
                    BT_CompositeNode compositeNode = decoratorView.parentView.node as BT_CompositeNode;
                    if (compositeNode != null)
                    {
                        Undo.RecordObject(compositeNode, "Undo delete decorator");
                        compositeNode.decorators.Remove(decoratorView.node as BT_Decorator);
                        EditorUtility.SetDirty(compositeNode);
                    }
                    // Remove decorator from behavior tree
                    tree.DestroyNode(decoratorView.node);
                    PopulateView();
                }

                BT_ServiceView serviceView = BehaviorTreeManager.selectedObject as BT_ServiceView;
                if (serviceView != null)
                {
                    // Remove service view from action node
                    BT_ActionNode actionNode = serviceView.parentView.node as BT_ActionNode;
                    if (actionNode != null)
                    {
                        Undo.RecordObject(actionNode, "Undo delete service");
                        actionNode.services.Remove(serviceView.node as BT_Service);
                        EditorUtility.SetDirty(actionNode);
                    }

                    // Remove service view from composite node
                    BT_CompositeNode compositeNode = serviceView.parentView.node as BT_CompositeNode;
                    if (compositeNode != null)
                    {
                        Undo.RecordObject(compositeNode, "Undo delete decorator");
                        compositeNode.services.Remove(serviceView.node as BT_Service);
                        EditorUtility.SetDirty(compositeNode);
                    }
                    // Remove service from behavior tree
                    tree.DestroyNode(serviceView.node);
                    PopulateView();
                }

            }
        }
        
        ///<summary>
        /// Called when the graph gets selected by the user.
        ///</summary>
        private void OnGraphSelected(MouseDownEvent evt)
        {
            BT_ChildNodeView btChildView = BehaviorTreeManager.selectedObject
                                                   as BT_ChildNodeView;
            if (btChildView != null)
            {
                btChildView.OnUnselected();
            }
        }
        
        ///<summary>
        /// Called when there is an undo/redo event.
        ///</summary>
        private void OnUndoRedo()
        {
            AssetDatabase.SaveAssets();
            PopulateView();
        }
        
        ///<summary>
        /// Find a node view by it's node.
        ///</summary>
        ///<param name="node"> The node contained in the node view you want to search</param>
        ///<returns> Return the node view of the given node </returns>
        public BT_NodeView FindNodeView(BT_Node node)
        {
            return GetNodeByGuid(node.guid.ToString()) as BT_NodeView;
        }
        
        ///<summary>
        /// Populate the graph by drawing all the nodes and links
        /// between them.
        ///</summary>
        public void PopulateView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;
            
            // If not already present, create the root node
            if (tree.rootNode == null && AssetDatabase.Contains(tree))
            {
                CreateNode(typeof(BT_RootNode), Vector2.zero);
            }
            
            // Create node views for all parent nodes.
            // N.B. child node views are directly created by parents
            // after their creation, see NodeFactory.cs CreateNodeView for more info.
            foreach (BT_Node node in tree.nodes)
            {
                // Is the node a parent node?
                if (node.GetType().IsSubclassOf(typeof(BT_ParentNode)))
                {
                    BT_NodeView view = NodeFactory.CreateNodeView(node, this);
                    // Setup selection callback on the node view to be the same
                    view.onNodeSelected += onNodeSelected;
                    AddElement(view);
                }
            }
            
            // Add edge links between nodes when we open a behavior tree asset graph 
            CreateNodesConnections();
        }
        
        // Create connections between all the nodes placed inside the behavior tree
        // graph.
        private void CreateNodesConnections()
        {
            // Add edge links between nodes when we open a behavior tree asset graph 
            foreach (BT_Node node in tree.nodes)
            {
                BT_ParentNodeView parentView = FindNodeView(node) as BT_ParentNodeView;
                BT_RootNode rootNode = node as BT_RootNode;
                if (rootNode != null && rootNode.childNode != null)
                {
                    // Connect root node to it's child.
                    BT_ParentNodeView childView = FindNodeView(rootNode.childNode) as BT_ParentNodeView;
                    CreateEdge(parentView, childView);
                }
                else
                {
                    List<BT_Node> childrens = tree.GetChildrenNodes(node);
                    // Connect all other nodes.
                    foreach (BT_Node childrenNode in childrens)
                    {
                        BT_ParentNodeView childView = FindNodeView(childrenNode) as BT_ParentNodeView;
                        CreateEdge(parentView, childView);
                    }
                }
            }
        }
        
        ///<summary>
        /// Create edge link between to two nodes
        ///</summary>
        private void CreateEdge(BT_ParentNodeView parentView, BT_ParentNodeView childView)
        {
            Edge edge = parentView.output.ConnectTo(childView.input);
            AddElement(edge);
        }

        ///<summary>
        /// Handle node and node visual element selection
        ///</summary>
        public override void AddToSelection(ISelectable selectable)
        {
            if (BehaviorTreeManager.hoverObject == null
               || BehaviorTreeManager.hoverObject.GetType() != typeof(BT_DecoratorView)
                && BehaviorTreeManager.hoverObject.GetType() != typeof(BT_ServiceView))
            {
                base.AddToSelection(selectable);
            }
        }

        ///<summary>
        /// Display contextual menu to track mouse position, display and handle nodes creation.
        ///</summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Workaround to find the mouse position in the graph space because evt.originalMousePosition doesn't work
            RegisterCallback<MouseDownEvent>
            (evt =>
            {
                mousePosition = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
            });
            
            // Has the user selected a node view object?
            if (BehaviorTreeManager.selectedObject == null)
            {
                // Get all parent nodes types in the project.
                TypeCache.TypeCollection parentTypes = TypeCache.GetTypesDerivedFrom<BT_ParentNode>();
            
                // For each parent node type create an action which allows developers to create
                // parent nodes in the graph at mouse position.
                foreach (Type nodeType in parentTypes)
                {
                    if (nodeType.BaseType != null)
                    {
                        string baseTypeName = nodeType.BaseType.Name.Remove(0, 3);
                        string actionName = baseTypeName + "/" + nodeType.Name;
                        evt.menu.AppendAction(actionName, (a) => CreateNode(nodeType, mousePosition));
                    }
                }
            
                // If there's no root node in the graph allow user to create it.
                if (tree.rootNode == null)
                {
                    evt.menu.AppendAction("Root", (a) => CreateNode(typeof(BT_RootNode), mousePosition));
                }
            }
        }
        
        ///<summary>
        /// Called each time the behavior tree view changes to update it.
        ///</summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // Updates nodes by removing from the graph view the deleted nodes
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement Element in graphViewChange.elementsToRemove)
                {
                    BT_NodeView NodeView = Element as BT_NodeView;
                    if (NodeView != null)
                    {
                        // Perform node destruction process
                        tree.DestroyNode(NodeView.node);
                    }

                    Edge edge = Element as Edge;
                    if (edge != null)
                    {
                        // The parent node is the node which is trying to connect to another node
                        BT_ParentNodeView ParentNode = edge.output.node as BT_ParentNodeView;
                        // The child node is the target node for the connection
                        BT_ParentNodeView ChildNode = edge.input.node as BT_ParentNodeView;
                        ChildNode.parentView = null;
                        tree.RemoveChildFromParent(ChildNode.node, ParentNode.node);
                        ParentNode.SortChildrenNodes();
                    }
                }
            }

            // Update nodes by adding the Child node to the Parent node
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    // The parent node it's node which is trying to connect to another node
                    BT_ParentNodeView parentNode = edge.output.node as BT_ParentNodeView;
                    // The child node it's the target node for the connection
                    BT_ParentNodeView childNode = edge.input.node as BT_ParentNodeView;
                    tree.AddChildToParentNode(childNode.node, parentNode.node);
                    parentNode.SortChildrenNodes();
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (Node node in nodes)
                {
                    BT_ParentNodeView nodeView = node as BT_ParentNodeView;
                    if(nodeView != null)
                        nodeView.SortChildrenNodes();
                }
            }
            return graphViewChange;
        }

        ///<summary>
        /// Get a list of compatible ports.
        ///</summary>
        ///<returns> A list of compatible ports</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction
            && endPort.node != startPort.node).ToList();
        }
        
        ///<summary>
        /// Create a node and display it in the graph
        ///</summary>
        ///<param name="type"> The type of the node you want to create</param>
        ///<param name="nodePosition"> The position of the node in the graph</param>
        private void CreateNode(Type type, Vector2 nodePosition)
        {
            BT_Node node = tree.CreateNode(type);
            node.position = nodePosition;
            BT_NodeView nodeView = NodeFactory.CreateNodeView(node, this);
            
            // Setup selection callback on the node view to be the same
            nodeView.onNodeSelected += onNodeSelected;
            AddElement(nodeView);
        }
        
        /// <summary>
        /// Create a child node attached to it's parent.
        /// </summary>
        /// <param name="nodeType"> The type of the child node to create. </param>
        /// <param name="btParentNode"> The parent to which the new child will be attached. </param>
        public void CreateChildNode(Type nodeType, BT_ParentNodeView btParentNode)
        {
            BT_ChildNode childNode = NodeFactory.CreateChildNode(nodeType, (BT_ParentNode) btParentNode.node, tree) as BT_ChildNode;
            BT_ChildNodeView nodeView = NodeFactory.CreateChildNodeView(btParentNode, childNode, this);
            
            // Setup selection callback on the node view to be the same
            nodeView.selectedCallback += onNodeVisualElementSelected;
        }
    }
}

