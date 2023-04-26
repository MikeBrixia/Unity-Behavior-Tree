using System;
using System.Collections.Generic;
using BT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    public abstract class BT_ParentNodeView : Node
    {
        
        ///<summary>
        /// Reference to the node encapsulated inside this node view, this value is going
        /// to contain the actual instructions of the node
        ///</summary>
        public BT_ParentNode node { get; }
        
        ///<summary>
        /// The parent view at which this node view is connected with
        /// it's input port.
        ///</summary>
        public BT_ParentNodeView parentView;
        
        ///<summary>
        /// Called when this node view gets selected by the user
        ///</summary>
        public Action<BT_ParentNodeView> onNodeSelected;
        
        /// <summary>
        /// Unique GUID identifier for this node.
        /// </summary>
        private GUID guid;

        ///<summary>
        /// The graph which owns this node
        ///</summary>
        protected readonly BehaviorTreeGraphView graph;
        
        ///<summary>
        /// Output port of the node
        ///</summary>
        public Port output { get; private set; }

        ///<summary>
        /// Input port of the node
        ///</summary>
        public Port input { get; private set; }
        
        ///<summary>
        /// The position of the mouse.
        ///</summary>
        private Vector2 mousePosition;
        
        protected BT_ParentNodeView(BT_ParentNode node, BehaviorTreeGraphView graph, string path) : base(path)
        {
            this.viewDataKey = node.guid.ToString();
            this.node = node;
            this.graph = graph;
            
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseEnter);
            
            // Set node position in the graph to where the user has clicked
            // to open the contextual menu
            Rect rect = this.contentRect;
            rect.position = this.node.position;
            SetPosition(rect);
            
            // Begin creating parent node GUI.
            OnCreateGUI();
        }

        protected void OnCreateGUI()
        {
            // Initialize all UI elements.
            InitializeUIElements();
            
            // CreateNodePorts parent node ports.
            CreateNodePorts();
        }
        
        private void OnMouseEnter(MouseLeaveEvent evt)
        {
            BehaviorTreeManager.hoverObject = null;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            BehaviorTreeManager.hoverObject = this;
        }
        
        ///<summary>
        /// Called when we initialize visual element.
        ///</summary>
        protected abstract void InitializeUIElements();
        
        /// <summary>
        /// Get all the child views of the parent
        /// </summary>
        /// <typeparam name="T"> The type of the child views </typeparam>
        /// <returns> A list of all child views inside the parent. </returns>
        public abstract List<T> GetChildViews<T>() where T : BT_ChildNodeView;
        
        /// <summary>
        /// Add a child view to this parent view.
        /// </summary>
        /// <param name="childView"> The child you want to add. </param>
        /// <typeparam name="T"> The type of the child view. </typeparam>
        public abstract void AddChildView<T>(T childView) where T : BT_ChildNodeView;
        
        /// <summary>
        /// Create all child views using child nodes.
        /// </summary>
        public abstract void CreateChildViews();
        
        ///<summary>
        /// Called when this node view gets selected.
        ///</summary>
        public override void OnSelected()
        {
            BehaviorTreeManager.selectedObject = this;
            onNodeSelected?.Invoke(this);
        }
        
        ///<summary>
        /// Called when this node view gets unselected.
        ///</summary>
        public override void OnUnselected()
        {
            BehaviorTreeManager.selectedObject = null;
        }

        /// <summary>
        ///  Show or hide node border.
        /// </summary>
        /// <param name="element"> The visual element you want to show the border. </param>
        /// <param name="width">the width of node border</param>
        protected void ShowSelectionBorder(VisualElement element, float width)
        {
            element.style.color = Color.blue;
            element.style.borderRightWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderTopWidth = width;
            element.style.borderBottomWidth = width;
        }
        
        ///<summary>
        /// Set the position of this node view.
        ///</summary>
        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            
            // update node position in the graph with
            // Undo/redo support. User will be able to undo
            // all nodes movement operations.
            Undo.RecordObject(node, "Node position");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        ///<summary>
        /// Create input port for this node view
        ///</summary>
        private void CreateInputPort()
        {
            if (node.GetType() != typeof(BT_RootNode))
            {
                // Create input port
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, null);
                input.style.flexDirection = FlexDirection.Column;
                // Add ports to their respective container
                inputContainer.Add(input);
            }
        }
        
        ///<summary>
        /// Create output port for this node view
        ///</summary>
        private void CreateOutputPort()
        {
            if (!node.GetType().IsSubclassOf(typeof(BT_ActionNode)))
            {
                Port.Capacity portCapacity = node.GetType().IsSubclassOf(typeof(BT_CompositeNode)) ? Port.Capacity.Multi : Port.Capacity.Single;

                // Create output port
                output = InstantiatePort(Orientation.Vertical, Direction.Output, portCapacity, null);
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }
        
        ///<summary>
        /// Create node input and output ports and initialize them.
        ///</summary>
        private void CreateNodePorts()
        {
            CreateInputPort();
            CreateOutputPort();
            RefreshExpandedState();
        }

        ///<summary>
        /// Sort children nodes from left to right.
        ///</summary>
        public void SortChildrenNodes()
        {
            BT_CompositeNode compositeNode = node as BT_CompositeNode;
            if (compositeNode != null)
            {
                compositeNode.children.Sort(SortByPosition);
            }
        }
        
        private int SortByPosition(BT_Node left, BT_Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }
        
        ///<summary>
        /// Create contextual menu to handle node visual element creation.
        ///</summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            
            // Is the node wrapped inside this view a root node?
            if (node.GetType() != typeof(BT_RootNode))
            {
                // Search for all Child node derived types
                var childTypes = TypeCache.GetTypesDerivedFrom<BT_ChildNode>();
                
                // For each child node type in the project create an action
                // which allows developers to create child nodes and attach them
                // to this node view.
                foreach (Type type in childTypes)
                {
                    if (type.BaseType != null & node != null & !type.IsAbstract)
                    {
                        string actionName = type.BaseType.Name + "/" + type.Name;
                        // Remove the BT_ prefix from the node type name
                        actionName = actionName.Remove(0, 3);
                        evt.menu.AppendAction(actionName, (a) => 
                            graph.CreateChildNode(type, this));
                    }
                }
            }
        }
    }
}