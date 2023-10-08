using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections;
using System.Linq;
using BT.Runtime;
using BT.Editor;
using Unity.Plastic.Antlr3.Runtime.Misc;
using Unity.Plastic.Newtonsoft.Json;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace BT
{
    ///<summary>
    /// The behavior tree graph view in which the user is going to
    /// Create, move and delete behavior tree nodes
    ///</summary>
    public sealed class BehaviorTreeGraphView : GraphView
    {
        
        public new class UxmlFactory : UxmlFactory<BehaviorTreeGraphView, UxmlTraits> { }
        
        ///<summary>
        /// The tree the graph is currently focusing on
        ///</summary>
        public BehaviorTree tree;

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
        /// the position of the mouse in the graph
        ///</summary>
        private Vector2 currentMousePosition;
        
        /// <summary>
        /// Used by the behavior tree graph to copy/paste nodes
        /// </summary>
        private readonly BehaviorTreeCopyPaster copyPaster;
        
        public BehaviorTreeGraphView()
        {
            // Insert background under everything else
            Insert(0, new GridBackground());

            // Load style sheet
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Editor/GridBackgroundStyle.uss");
            styleSheets.Add(styleSheet);
            
            // Add manipulators to this graph
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            // Setup custom undo/redo events
            Undo.undoRedoPerformed += OnUndoRedo;

            // Keyboard callbacks
            RegisterCallback<KeyDownEvent>(OnKeyboardPressed, TrickleDown.TrickleDown);
            
            // Mouse callbacks
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            
            // Copy/paste callbacks
            serializeGraphElements += OnCopy;
            unserializeAndPaste += OnPaste;

            copyPaster = new BehaviorTreeCopyPaster(this);
        }

        /// <summary>
        /// Called when the mouse pointer moves inside the behavior tree graph.
        /// </summary>
        /// <param name="evt"></param>
        private void OnPointerMove(PointerMoveEvent evt)
        {
            // Update mouse position.
            currentMousePosition = ((Vector2)evt.localPosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
        }
        
        ///<summary>
        /// Called when the user express the intention to paste some nodes(ctrl-v)
        ///</summary>
        private void OnPaste(string operationName, string data)
        {
            // Remove all copied node in favor of the new selected elements.
            copyPaster.PasteNodes(currentMousePosition);
            
            // Repopulate view with new pasted nodes.
            PopulateView();
        }

        ///<summary>
        /// Called when the user express the intention to copy some nodes(ctrl-c)
        ///</summary>
        private string OnCopy(IEnumerable<GraphElement> elements)
        {
            var nodeViews = new List<BT_ParentNodeView>();
            foreach (GraphElement element in elements)
            {
                if (element is BT_ParentNodeView view)
                {
                    nodeViews.Add(view);
                }
            }
            
            // Copy the nodes.
            copyPaster.CopyNodes(nodeViews);
            
            return "None";
        }

        ///<summary>
        /// Called each time there is a keyboard event inside the graph.
        ///</summary>
        private void OnKeyboardPressed(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                // Otherwise destroy the child node and remove it from it's parent and from the tree.
                if (BehaviorTreeManager.selectedObject is BT_ChildNodeView childView)
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
        /// Called when there is an undo/redo event.
        ///</summary>
        private void OnUndoRedo()
        {
            tree = Selection.activeObject as BehaviorTree;
            AssetDatabase.SaveAssets();
            PopulateView();
        }
        
        ///<summary>
        /// Find a node view by it's node.
        ///</summary>
        ///<param name="node"> The node contained inside the view you want to search</param>
        ///<returns> Return the node view of the given node </returns>
        public BT_ParentNodeView FindNodeView(BT_Node node)
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
                // Use node factory to create the missing root node.
                BT_RootNode rootNode = NodeFactory.CreateNode<BT_RootNode>(tree);
                rootNode.position = Vector2.zero;
            }
            
            // Create node views for all parent nodes.
            // N.B. child node views are directly created by parents
            // after their creation, see NodeFactory.cs CreateNodeView for more info.
            foreach (BT_Node node in tree.nodes)
            {
                // Is the node a parent node?
                if (node.GetType().IsSubclassOf(typeof(BT_ParentNode)))
                {
                    CreateNodeView(node);
                    Debug.Log(node);
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
                else if(node is BT_ParentNode parentNode and not BT_RootNode)
                {
                    // Connect all other nodes.
                    foreach (BT_ParentNode childrenNode in parentNode.GetConnectedNodes())
                    {
                        BT_ParentNodeView childView = FindNodeView(childrenNode);
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
        
        public override void AddToSelection(ISelectable selectable)
        {
            if (selectable is BT_ChildNodeView && selection.Count <=1)
            {
                ClearSelection();
            }
            base.AddToSelection(selectable);
        }
        
        ///<summary>
        /// Display contextual menu to track mouse position, display and handle nodes creation.
        ///</summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Has the user selected a node view object?
            if (BehaviorTreeManager.selectedObject == null)
            {
                // Compute mouse position(evt.originalMousePosition returns strange position).
                currentMousePosition = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
                
                // Get all parent nodes types in the project.
                TypeCache.TypeCollection parentTypes = TypeCache.GetTypesDerivedFrom<BT_ParentNode>();
            
                // For each parent node type create an action which allows developers to create
                // parent nodes in the graph at mouse position.
                foreach (Type nodeType in parentTypes)
                {
                    if (nodeType.BaseType != null && nodeType.BaseType != typeof(BT_ParentNode))
                    {
                        string baseTypeName = nodeType.BaseType.Name.Remove(0, 3);
                        string actionName = baseTypeName + "/" + nodeType.Name;
                        evt.menu.AppendAction(actionName, (a) => CreateNode(nodeType, currentMousePosition));
                    }
                }
            
                // If there's no root node in the graph allow user to create it.
                if (tree.rootNode == null)
                {
                    evt.menu.AppendAction("Root", (a) => CreateNode(typeof(BT_RootNode), currentMousePosition));
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
                        nodeView.SortChildrenNodesByHorizontalPosition();
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
                        NodeFactory.DestroyParentNode(parentNodeView.node, tree);
                    }
                    
                    // Update connected nodes.
                    if (element is Edge edge)
                    {
                        // The parent node is the node which is trying to connect to another node
                        BT_ParentNodeView parentNode = (BT_ParentNodeView) edge.output.node;
                        // The child node is the target node for the connection
                        BT_ParentNodeView childNode = (BT_ParentNodeView) edge.input.node;

                        // Remove connected child from parent and register and undo/redo action for it.
                        Undo.RecordObject(parentNode.node, "Behavior Tree Composite Node remove child");
                        parentNode.node.DisconnectNode(childNode.node);
                        EditorUtility.SetDirty(parentNode.node);
                        
                        // Once an element has been removed, re-sort all child nodes
                        // to determine the correct order of execution
                        parentNode.SortChildrenNodesByHorizontalPosition();
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
                    BT_ParentNodeView parentNode = (BT_ParentNodeView) edge.output.node;
                    // The child node it's the target node for the connection
                    BT_ParentNodeView childNode = (BT_ParentNodeView) edge.input.node;
                    
                    // Add child parent node to parent.
                    Undo.RecordObject(parentNode.node, "Behavior Tree Composite Node add child");
                    parentNode.node.ConnectNode(childNode.node);
                    EditorUtility.SetDirty(parentNode.node);
                    
                    // Once added, sort all the parent children.
                    parentNode.SortChildrenNodesByHorizontalPosition();
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
        ///<param name="type"> The type of the node you want to create </param>
        ///<param name="nodePosition"> The position of the node in the graph </param>
        private BT_Node CreateNode(Type type, Vector2 nodePosition)
        {
            // Create a new node of the supplied type.
            BT_Node node = NodeFactory.CreateNode(type, tree);
            node.position = nodePosition;
            
            // Wrap created node inside a node view.
            CreateNodeView(node);
            
            return node;
        }
        
        /// <summary>
        /// Create a node view for the supplied behavior tree node.
        /// </summary>
        /// <param name="node"> Teh node which will be wrapped inside the node view.</param>
        private BT_ParentNodeView CreateNodeView(BT_Node node)
        {
            BT_ParentNodeView parentNodeView = NodeFactory.CreateNodeView(node, this);
            
            // Setup selection callback on the node view to be the same
            parentNodeView.onNodeSelected += onNodeSelected;
            AddElement(parentNodeView);

            return parentNodeView;
        }
        
        /// <summary>
        /// Create a child node attached to it's parent.
        /// </summary>
        /// <param name="nodeType"> The type of the child node to create. </param>
        /// <param name="btParentNode"> The parent to which the new child will be attached. </param>
        public BT_ChildNodeView CreateChildNode(Type nodeType, BT_ParentNodeView btParentNode)
        {
            BT_ChildNode childNode = NodeFactory.CreateChildNode(nodeType, btParentNode.node, tree) as BT_ChildNode;
            BT_ChildNodeView nodeView = NodeFactory.CreateChildNodeView(btParentNode, childNode, this);
            
            // Setup selection callback on the node view to be the same
            nodeView.selectedCallback += onChildNodeSelected;

            return nodeView;
        }
        
        /// <summary>
        /// Update live debugging for the behavior tree editor.
        /// </summary>
        /// <param name="behaviorTree"> The behavior tree instance to debug. </param>
        public void UpdateGraphLiveDebug(BehaviorTree behaviorTree)
        {
            // Reset edges appearance.
            foreach (Edge edge in edges)
            {
                edge.edgeControl.edgeWidth = 2;
                edge.edgeControl.inputColor = Color.white;
            }
            
            // Highlight all the edges connecting nodes which
            // are been executed.
            HighlightTreeExecutionEdges(behaviorTree);
        }

        private void HighlightTreeExecutionEdges(BehaviorTree behaviorTree)
        {
            // Ensure that the selected behavior tree asset is a clone of the currently
            // inspected behavior tree.
            if (behaviorTree.IsCloneOf(tree) && behaviorTree.rootNode != null)
            {
                // Initialize visit queue with the root node as the first node to visit.
                var toVisit = new Queue<BT_ParentNode>();
                toVisit.Enqueue(behaviorTree.rootNode);
                
                // Keep iterating as long as there are nodes to visit.
                while (toVisit.Count != 0)
                {
                    BT_ParentNode currentNode = toVisit.Dequeue();
                    foreach (BT_ParentNode child in currentNode.GetConnectedNodes())
                    {
                        // Is the current visited node a successful node?
                        if (child.state == ENodeState.Success)
                        {
                            // If true, add it to the queue of nodes to visit.
                            toVisit.Enqueue(child);
                            
                            // And highlight the edge connecting the node to it's parent.
                            BT_ParentNodeView currentNodeView = FindNodeView(child);
                            Edge connectionEdge = currentNodeView.input.connections.First();
                            connectionEdge.edgeControl.edgeWidth = 5;
                            connectionEdge.edgeControl.inputColor = Color.yellow;
                            
                            break;
                        }
                        else if (child.state == ENodeState.Running)
                        {
                            // And highlight the edge connecting the node to it's parent.
                            BT_ParentNodeView currentNodeView = FindNodeView(child);
                            Edge connectionEdge = currentNodeView.input.connections.First();
                            connectionEdge.edgeControl.edgeWidth = 5;
                            connectionEdge.edgeControl.inputColor = Color.yellow;
                            
                            // Jump out of the loops, if the node is currently running
                            // we've reached the destination.
                            goto exit_loop;
                        }
                        
                    }
                }

                exit_loop :;
            }
        }
    }
    
}

