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
        public Action<BT_NodeView> OnNodeSelected;

        ///<summary>
        /// Called when the user select a node visual element inside the graph.
        /// Node visual element are all the nodes which can be attached to other nodes.
        ///</summary>
        public Action<BT_NodeVisualElement> onNodeVisualElementSelected;
        
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
                BT_Node newNode = tree.CreateNode(copiedNode.node.GetType());
                newNode = copiedNode.node;
            }
            // Once we finished pasting nodes, clear the copy cache
            // and repopulate the view
            copyCache.Clear();
            PopulateView(tree);
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
                BT_DecoratorView decoratorView = BehaviorTreeSelectionManager.selectedObject as BT_DecoratorView;
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
                    PopulateView(tree);
                }

                BT_ServiceView serviceView = BehaviorTreeSelectionManager.selectedObject as BT_ServiceView;
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
                    PopulateView(tree);
                }

            }
        }
        
        ///<summary>
        /// Called when the graph gets selected by the user.
        ///</summary>
        private void OnGraphSelected(MouseDownEvent evt)
        {
            BT_NodeVisualElement btVisualElement = BehaviorTreeSelectionManager.selectedObject
                                                   as BT_NodeVisualElement;
            if (btVisualElement != null)
            {
                btVisualElement.OnUnselected();
            }
        }
        
        ///<summary>
        /// Called when there is an undo/redo event.
        ///</summary>
        private void OnUndoRedo()
        {
            AssetDatabase.SaveAssets();
            PopulateView(tree);
        }
        
        ///<summary>
        /// Find a node view by it's node.
        ///</summary>
        ///<param name="node"> The node contained in the node view you want to search<param>
        ///<returns> Return the node view of the given node </returns>
        public BT_NodeView FindNodeView(BT_Node Node)
        {
            return GetNodeByGuid(Node.guid.ToString()) as BT_NodeView;
        }
        
        ///<summary>
        /// Populate the graph by drawing all the nodes and links
        /// between them.
        ///</summary>
        ///<param name="tree"> The Behavior Tree you want to display in the graph </param>
        public void PopulateView(BehaviorTree tree)
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null && AssetDatabase.Contains(tree))
            {
                tree.rootNode = tree.CreateNode(typeof(BT_RootNode)) as BT_RootNode;
            }
            tree.nodes.ForEach(node => CreateNodeView(node));

            // Add edge links between nodes when we open a behavior tree asset graph 
            foreach (BT_Node Node in tree.nodes)
            {
                BT_NodeView ParentView = FindNodeView(Node);
                BT_RootNode rootNode = Node as BT_RootNode;
                if (rootNode != null && rootNode.childNode != null)
                {
                    BT_NodeView ChildView = FindNodeView(rootNode.childNode);
                    CreateEdge(ParentView, ChildView);
                }
                else
                {
                    List<BT_Node> Childrens = tree.GetChildrenNodes(Node);
                    foreach (BT_Node ChildrenNode in Childrens)
                    {
                        BT_NodeView ChildView = FindNodeView(ChildrenNode);
                        CreateEdge(ParentView, ChildView);
                    }
                }
            }
        }

        ///<summary>
        /// Create edgle between to two nodes
        ///</summary>
        private void CreateEdge(BT_NodeView parentView, BT_NodeView childView)
        {
            Edge edge = parentView.output.ConnectTo(childView.input);
            AddElement(edge);
        }

        ///<summary>
        /// Handle node and node visual element selection
        ///</summary>
        public override void AddToSelection(ISelectable selectable)
        {
            if (BehaviorTreeSelectionManager.hoverObject == null
               || BehaviorTreeSelectionManager.hoverObject.GetType() != typeof(BT_DecoratorView)
                && BehaviorTreeSelectionManager.hoverObject.GetType() != typeof(BT_ServiceView))
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

            // Display all composite nodes
            var compositeTypes = TypeCache.GetTypesDerivedFrom<BT_CompositeNode>();
            foreach (var type in compositeTypes)
            {
                evt.menu.AppendAction("Composite/" + type.Name, (a) => CreateNode(type, mousePosition));
            }

            // Display all action nodes
            var actionTypes = TypeCache.GetTypesDerivedFrom<BT_ActionNode>();
            foreach (var type in actionTypes)
            {
                evt.menu.AppendAction("Action/" + type.Name, (a) => CreateNode(type, mousePosition));
            }

            if (tree.rootNode == null)
            {
                evt.menu.AppendAction("Root", (a) => CreateNode(typeof(BT_RootNode), mousePosition));
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
                        // The parent node it's node which is trying to connect to another node
                        BT_NodeView ParentNode = edge.output.node as BT_NodeView;
                        // The child node it's the target node for the connection
                        BT_NodeView ChildNode = edge.input.node as BT_NodeView;
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
                    BT_NodeView ParentNode = edge.output.node as BT_NodeView;
                    // The child node it's the target node for the connection
                    BT_NodeView ChildNode = edge.input.node as BT_NodeView;
                    tree.AddChildToParentNode(ChildNode.node, ParentNode.node);
                    ParentNode.SortChildrenNodes();
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (Node node in nodes)
                {
                    BT_NodeView nodeView = node as BT_NodeView;
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
        public void CreateNode(Type type, Vector2 nodePosition)
        {
            BT_Node Node = tree.CreateNode(type);
            Node.position = nodePosition;
            CreateNodeView(Node);
        }

        ///<summary>
        /// Create a brand new node and attach it to a parent node
        ///</summary>
        ///<param name="nodeType"> The type of the node visual element you want to create and attach</param>
        ///<param name="parentNode"> The Parent node to which the created node will be attached</param>
        public void CreateAttachedNode(Type nodeType, BT_NodeView parentNode)
        {
            BT_Node node = tree.CreateNode(nodeType);

            BT_Decorator decoratorNode = node as BT_Decorator;
            if (decoratorNode != null)
            {
                AttachDecoratorToParent(decoratorNode, parentNode);
                CreateDecoratorViewAttached(decoratorNode, parentNode);
            }

            BT_Service serviceNode = node as BT_Service;
            if (serviceNode != null)
            {
                AttachServiceToParent(serviceNode, parentNode);
                CreateServiceViewAttached(serviceNode, parentNode);
            }
        }
        
        ///<summary>
        /// Attach a decorator node to a parent view
        ///</summary>
        ///<param name="decorator"> The decorator node you want to attach</param>
        ///<param name="parentNode"> The view to which the decorators will be attached</param>
        private void AttachDecoratorToParent(BT_Decorator decorator, BT_NodeView parentNode)
        {
            BT_CompositeNode compositeNode = parentNode.node as BT_CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Undo decorator creation");
                compositeNode.decorators.Add(decorator);
                EditorUtility.SetDirty(compositeNode);
            }

            BT_ActionNode actionNode = parentNode.node as BT_ActionNode;
            if (actionNode != null)
            {
                Undo.RecordObject(actionNode, "Undo decorator creation");
                actionNode.decorators.Add(decorator);
                EditorUtility.SetDirty(actionNode);
            }
        }
        
        ///<summary>
        /// Attach a service node to a parent view
        ///</summary>
        ///<param name="service"> The service node you want to attach</param>
        ///<param name="parentNode"> The view to which the service will be attached</param>
        private void AttachServiceToParent(BT_Service service, BT_NodeView parentNode)
        {
            BT_CompositeNode compositeNode = parentNode.node as BT_CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Undo decorator creation");
                compositeNode.services.Add(service);
                EditorUtility.SetDirty(compositeNode);
            }

            BT_ActionNode actionNode = parentNode.node as BT_ActionNode;
            if (actionNode != null)
            {
                Undo.RecordObject(actionNode, "Undo decorator creation");
                actionNode.services.Add(service);
                EditorUtility.SetDirty(actionNode);
            }
        }

        ///<summary>
        /// Create a decorator view and attach it to a parent node view
        ///</summary>
        ///<param name="decorator"> The decorator node to attach</param>
        ///<param name="parentNodeView"> The view to which the decorator it's going to be attacched</param>
        ///<param name="filepath"> The filepath of the decorator view uxml to render on screen.</param>
        public void CreateDecoratorViewAttached(BT_Decorator decorator, BT_NodeView parentNodeView,
                                                string filepath = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/DecoratorView.uxml")
        {
            BT_DecoratorView decoratorView = new BT_DecoratorView(parentNodeView, decorator, filepath);
            decoratorView.selectedCallback += onNodeVisualElementSelected;
        }

        ///<summary>
        /// Create a service view and attach it to a parent node view
        ///</summary>
        ///<param name="service"> The decorator node to attach</param>
        ///<param name="parentNodeView"> The view to which the service it's going to be attacched</param>
        ///<param name="filepath"> The filepath of the service view uxml to render on screen.</param>
        public void CreateServiceViewAttached(BT_Service service, BT_NodeView parentNodeView,
                                              string filepath = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ServiceView.uxml")
        {
            BT_ServiceView serviceView = new BT_ServiceView(parentNodeView, service, filepath);
            serviceView.selectedCallback += onNodeVisualElementSelected;
        }
        
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node from which the node view will be created </param>
        private void CreateNodeView(BT_Node node)
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            BT_NodeView nodeView = new BT_NodeView(node, this);

            // Create decorators views for composite and action nodes
            if (node.GetType().IsSubclassOf(typeof(BT_CompositeNode)))
            {
                BT_CompositeNode compositeNode = node as BT_CompositeNode;
                compositeNode.decorators.ForEach(decorator => CreateDecoratorViewAttached(decorator, nodeView));
                compositeNode.services.ForEach(service => CreateServiceViewAttached(service, nodeView));
            }
            else if (node.GetType().IsSubclassOf(typeof(BT_ActionNode)))
            {
                BT_ActionNode actionNode = node as BT_ActionNode;
                actionNode.decorators.ForEach(decorator => CreateDecoratorViewAttached(decorator, nodeView));
                actionNode.services.ForEach(service => CreateServiceViewAttached(service, nodeView));
            }

            // Setup selection callback on the node view to be the same
            nodeView.OnNodeSelected += OnNodeSelected;
            AddElement(nodeView);
        }
    }
}

