using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using UnityEngine.EventSystems;

namespace BT
{
    public class BehaviorTreeGraphView : GraphView
    {

        public new class UxmlFactory : UxmlFactory<BehaviorTreeGraphView, UxmlTraits> { }

        public BehaviorTree Tree;

        ///<summary>
        /// the position of the mouse in the graph
        ///</summary>
        private Vector2 mousePosition;

        public Action<BT_NodeView> OnNodeSelected;
        public Action<BT_NodeVisualElement> onNodeVisualElementSelected;

        private EventCallback<MouseDownEvent> mousePressedEvent;

        public BehaviorTreeGraphView()
        {
            // Insert background under everything else
            Insert(0, new GridBackground());

            // Load style sheet
            var StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UnityBehaviorTreeSystem/Editor/BehaviorTree/GridBackgroundStyle.uss");
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

            // Place default root node here...
        }

        // Handles decorator node and view destruction correctly
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
                    Tree.DestroyNode(decoratorView.node);
                    PopulateView(Tree);
                }

                BT_ServiceView serviceView = BehaviorTreeSelectionManager.selectedObject as BT_ServiceView;
                if(serviceView != null)
                {
                    BT_ActionNode actionNode = serviceView.parentView.node as BT_ActionNode;
                    if(actionNode != null)
                    {
                        Undo.RecordObject(actionNode, "Undo delete service");
                        actionNode.services.Remove(serviceView.node as BT_Service);
                        EditorUtility.SetDirty(actionNode);
                    }

                    // Remove decorator view from composite node
                    BT_CompositeNode compositeNode = serviceView.parentView.node as BT_CompositeNode;
                    if (compositeNode != null)
                    {
                        Undo.RecordObject(compositeNode, "Undo delete decorator");
                        compositeNode.services.Remove(serviceView.node as BT_Service);
                        EditorUtility.SetDirty(compositeNode);
                    }
                    // Remove service from behavior tree
                    Tree.DestroyNode(serviceView.node);
                    PopulateView(Tree);
                }
                
            }
        }

        private void OnGraphSelected(MouseDownEvent evt)
        {
            Debug.Log(BehaviorTreeSelectionManager.selectedObject);
            BT_NodeVisualElement btVisualElement = BehaviorTreeSelectionManager.selectedObject
                                                   as BT_NodeVisualElement;
            if (btVisualElement != null)
            {
                btVisualElement.OnUnselected();
            }
        }

        private void OnUndoRedo()
        {
            AssetDatabase.SaveAssets();
            PopulateView(Tree);
        }
        
        public BT_NodeView FindNodeView(BT_Node Node)
        {
            return GetNodeByGuid(Node.guid.ToString()) as BT_NodeView;
        }

        public void PopulateView(BehaviorTree tree)
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode is null && AssetDatabase.Contains(tree))
            {
                tree.rootNode = tree.CreateNode(typeof(BT_RootNode)) as BT_RootNode;
                EditorUtility.SetDirty(tree);
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

        // Create an edge
        private void CreateEdge(BT_NodeView parentView, BT_NodeView childView)
        {
            Edge edge = parentView.output.ConnectTo(childView.input);
            AddElement(edge);
        }

        // Handle nodes selection in the graph
        public override void AddToSelection(ISelectable selectable)
        {
            if (BehaviorTreeSelectionManager.hoverObject == null
               || BehaviorTreeSelectionManager.hoverObject.GetType() != typeof(BT_DecoratorView)
                && BehaviorTreeSelectionManager.hoverObject.GetType() != typeof(BT_ServiceView))
            {
                base.AddToSelection(selectable);
            }
        }

        // Called when the user want to open the Contextual menu in the behavior tree graph
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
            
            if(Tree.rootNode == null)
            {
                evt.menu.AppendAction("Root", (a) => CreateNode(typeof(BT_RootNode), mousePosition));
            }
        }
        
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
                        Tree.DestroyNode(NodeView.node);
                    }

                    Edge edge = Element as Edge;
                    if (edge != null)
                    {
                        // The parent node it's node which is trying to connect to another node
                        BT_NodeView ParentNode = edge.output.node as BT_NodeView;
                        // The child node it's the target node for the connection
                        BT_NodeView ChildNode = edge.input.node as BT_NodeView;
                        Tree.RemoveChildFromParent(ChildNode.node, ParentNode.node);
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
                    Tree.AddChildToParentNode(ChildNode.node, ParentNode.node);
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

        // Handle input/output linkage for nodes
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction
            && endPort.node != startPort.node).ToList();
        }

        public void CreateNode(Type type, Vector2 NodePosition)
        {
            BT_Node Node = Tree.CreateNode(type);
            Node.position = NodePosition;
            CreateNodeView(Node);
        }

        ///<summary>
        /// Create a brand new node and attach it to a parent node
        ///</summary>
        public void CreateAttachedNode(Type nodeType, BT_NodeView parentNode)
        {
            BT_Node node = Tree.CreateNode(nodeType);

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
        public void CreateDecoratorViewAttached(BT_Decorator decorator, BT_NodeView parentNodeView, 
                                                string filepath = "Assets/UnityBehaviorTreeSystem/Editor/BehaviorTree/BT Elements/DecoratorView.uxml")
        {
            BT_DecoratorView decoratorView = new BT_DecoratorView(parentNodeView, decorator, filepath);
            decoratorView.selectedCallback += onNodeVisualElementSelected;
        }
        
        ///<summary>
        /// Create a service view and attach it to a parent node view
        ///</summary>
        public void CreateServiceViewAttached(BT_Service service, BT_NodeView parentNodeView, 
                                              string filepath = "Assets/UnityBehaviorTreeSystem/Editor/BehaviorTree/BT Elements/ServiceView.uxml")
        {
            BT_ServiceView serviceView = new BT_ServiceView(parentNodeView, service, filepath);
            serviceView.selectedCallback += onNodeVisualElementSelected;
        }

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

