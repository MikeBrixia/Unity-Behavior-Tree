
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using BT.Runtime;
using BT.Editor;

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
        public Action<BT_ParentNodeView> onNodeSelected;

        ///<summary>
        /// Called when the user select a node visual element inside the graph.
        /// Node visual element are all the nodes which can be attached to other nodes.
        ///</summary>
        public Action<BT_ChildNodeView> onChildNodeSelected;
        
        ///<summary>
        /// Called when the user press a mouse button while it's cursor is inside
        /// the graph window.
        ///</summary>
        private EventCallback<MouseDownEvent> mousePressedEvent;

        ///<summary>
        /// all the data which we want to copy with CTRL-C
        ///</summary>
        private List<BT_ParentNodeView> copyCache = new List<BT_ParentNodeView>();

        public BehaviorTreeGraphView()
        {
            // Insert background under everything else
            Insert(0, new GridBackground());

            // Load style sheet
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ai.behavior-tree/Editor/BehaviorTree/GridBackgroundStyle.uss");
            styleSheets.Add(styleSheet);

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
            foreach (BT_ParentNodeView copiedNode in copyCache)
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
                BT_ParentNodeView parentNodeToCopy = element as BT_ParentNodeView;
                if (parentNodeToCopy != null)
                {
                    copyCache.Add(parentNodeToCopy);
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
                // If a parent node, destroy it.
                if (BehaviorTreeManager.selectedObject is BT_ParentNodeView parentView)
                {
                    // Remove decorator from behavior tree
                    NodeFactory.DestroyParentNode(parentView.node, tree);
                    PopulateView();
                }
                // Otherwise destroy the child node and remove it from it's parent and from the tree.
                else if (BehaviorTreeManager.selectedObject is BT_ChildNodeView childView)
                {
                    BT_ParentNodeView pView = childView.parentView;
                    
                    // Remove node from parent.
                    Undo.RecordObject(pView.node, "Undo delete decorator");
                    pView.node.DestroyChild(childView.node);
                    EditorUtility.SetDirty(pView.node);
                    
                    // Destroy the child node and remove it from the tree.
                    NodeFactory.DestroyChildNode(childView.parentView.node, childView.node, tree);
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
        private BT_ParentNodeView FindNodeView(BT_Node node)
        {
            return GetNodeByGuid(node.guid.ToString()) as BT_ParentNodeView;
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
                    BT_ParentNodeView view = NodeFactory.CreateNodeView(node, this);
                    AddElement(view);
                }
            }
            
            // Add edge links between nodes when we open a behavior tree asset graph 
            CreateNodesConnections();
        }
        
        /// <summary>
        /// Create connections between all the nodes placed inside the behavior tree
        /// graph.
        /// </summary>
        private void CreateNodesConnections()
        {
            // Add edge links between nodes when we open a behavior tree asset graph 
            foreach (BT_Node node in tree.nodes)
            {
                BT_ParentNodeView parentView = FindNodeView(node);
                BT_RootNode rootNode = node as BT_RootNode;
                if (rootNode != null && rootNode.childNode != null)
                {
                    // Connect root node to it's child.
                    BT_ParentNodeView childView = FindNodeView(rootNode.childNode);
                    CreateEdge(parentView, childView);
                }
                else if(node is BT_ParentNode parentNode)
                {
                    List<BT_Node> children = parentNode.GetChildNodes();

                    if (children != null)
                    {
                        // Connect all other nodes.
                        foreach (BT_Node childrenNode in children)
                        {
                            BT_ParentNodeView childView = FindNodeView(childrenNode);
                            CreateEdge(parentView, childView);
                        }
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
            
            // Remove all the nodes which have been deleted.
            RemoveDeletedNodes(graphViewChange);

            // Update nodes by adding the Child node to the Parent node
            RecreateEdges(graphViewChange);
            
            // Sort by position all the moved node elements.
            if (graphViewChange.movedElements != null)
            {
                foreach (Node node in nodes)
                {
                    if(node is BT_ParentNodeView nodeView)
                        nodeView.SortChildrenNodes();
                }
            }
            return graphViewChange;
        }
        
        /// <summary>
        /// Remove from the graph all the nodes which have been deleted.
        /// </summary>
        private void RemoveDeletedNodes(GraphViewChange graphViewChange)
        {
            // Updates nodes by removing from the graph view the deleted nodes
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is BT_ParentNodeView parentNodeView)
                    {
                        // Destroy the node.
                        NodeFactory.DestroyParentNode(parentNodeView.node, tree);
                    }
                    
                    // Update connected nodes.
                    if (element is Edge edge)
                    {
                        // The parent node is the node which is trying to connect to another node
                        BT_ParentNodeView parentNode = edge.output.node as BT_ParentNodeView;
                        // The child node is the target node for the connection
                        BT_ParentNodeView childNode = edge.input.node as BT_ParentNodeView;
                        childNode.parentView = null;
                        parentNode.SortChildrenNodes();
                    }
                }
            }
        }
        
        /// <summary>
        /// Recreate connections edges between graph nodes.
        /// </summary>
        /// <param name="graphViewChange"> Registered graph changes. </param>
        private void RecreateEdges(GraphViewChange graphViewChange)
        {
            // Update nodes by adding the Child node to the Parent node
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    // The parent node it's node which is trying to connect to another node
                    BT_ParentNodeView parentNode = edge.output.node as BT_ParentNodeView;
                    // The child node it's the target node for the connection
                    BT_ParentNodeView childNode = edge.input.node as BT_ParentNodeView;
                    
                    // Add child parent node to parent.
                    Undo.RecordObject(parentNode.node, "Behavior Tree Composite Node add child");
                    parentNode.node.AddChildNode(childNode.node);
                    EditorUtility.SetDirty(parentNode.node);
                    // Once added, sort all the parent children.
                    parentNode.SortChildrenNodes();
                }
            }
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
            BT_Node node = NodeFactory.CreateNode(type, tree);
            node.position = nodePosition;
            BT_ParentNodeView parentNodeView = NodeFactory.CreateNodeView(node, this);
            
            // Setup selection callback on the node view to be the same
            parentNodeView.onNodeSelected += onNodeSelected;
            AddElement(parentNodeView);
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
            nodeView.selectedCallback += onChildNodeSelected;
        }
    }
}

