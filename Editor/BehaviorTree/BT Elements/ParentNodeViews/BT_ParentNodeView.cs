using System;
using BT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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
        public BT_ParentNode node { get; protected set; }
        
        ///<summary>
        /// The parent view at which this node view is connected with
        /// it's input port.
        ///</summary>
        public BT_ParentNodeView parentView;
        
        ///<summary>
        /// Called when this node view gets selected by the user
        ///</summary>
        public Action<BT_ParentNodeView> onNodeSelected;
        
        ///<summary>
        /// Container for decorator nodes
        ///</summary>
        public VisualElement decoratorsContainer { get; private set; }

        ///<summary>
        /// Container service containers
        ///</summary>
        public VisualElement serviceContainer { get; private set; }
        
        /// <summary>
        /// Unique GUID identifier for this node.
        /// </summary>
        private GUID guid;

        ///<summary>
        /// The graph which owns this node
        ///</summary>
        public BehaviorTreeGraphView graph { get; private set; }
        
        ///<summary>
        /// The displayed node name
        ///</summary>
        private Label nodeNameLabel;
        
        /// <summary>
        /// The displayed node type name.
        /// </summary>
        private Label nodeTypeNameLabel;
        
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
        
        ///<summary>
        /// The displayed node description
        ///</summary>
        private Label nodeDescriptionLabel;
        private VisualElement titleElement;
        private VisualElement nodeBorder;

        protected BT_ParentNodeView(BT_ParentNode node, BehaviorTreeGraphView graph, string path) : base(path)
        {
            this.viewDataKey = node.guid.ToString();
            this.node = node;
            this.graph = graph;
            
            // Initialize all the UI elements.
            InitializeUIElements();
            
            // Set node position in the graph to where the user has clicked
            // to open the contextual menu
            Rect rect = this.contentRect;
            rect.position = this.node.position;
            SetPosition(rect);
            
            // Draw parent node ports.
            Draw();
            
            // Register mouse callbacks
            EventCallback<MouseEnterEvent> mouseEnterEvent = OnMouseEnter;
            RegisterCallback<MouseEnterEvent>(mouseEnterEvent);
        }
        
        ///<summary>
        /// Called when the mouse cursor enter this node view.
        ///</summary>
        private void OnMouseEnter(MouseEnterEvent evt)
        {
            BehaviorTreeManager.hoverObject = this;
        }
        
        ///<summary>
        /// Called when we initialize visual element.
        ///</summary>
        private void InitializeUIElements()
        {
            nodeNameLabel = mainContainer.parent.Q<Label>("NodeTitle");
            nodeTypeNameLabel = mainContainer.parent.Q<Label>("NodeTypeName");
            SerializedObject serializedNode = new SerializedObject(node);
            
            // Bind node name value to label
            nodeNameLabel.bindingPath = "nodeName";
            nodeNameLabel.Bind(serializedNode);
            
            // Bind node type name value to label
            nodeTypeNameLabel.bindingPath = "nodeTypeName";
            nodeTypeNameLabel.Bind(serializedNode);
            
            // Bind description value to description label.
            nodeDescriptionLabel = mainContainer.parent.Q<Label>("NodeDescription");
            nodeDescriptionLabel.bindingPath = "description";
            nodeDescriptionLabel.Bind(serializedNode);

            decoratorsContainer = mainContainer.parent.Q<VisualElement>("DecoratorsContainer");
            serviceContainer = mainContainer.parent.Q<VisualElement>("ServiceContainer");
            nodeBorder = mainContainer.parent.Q<VisualElement>("selection-border");
        }
        
        ///<summary>
        /// Called when this node view gets selected.
        ///</summary>
        public override void OnSelected()
        {
            BehaviorTreeManager.selectedObject = this;
            ShowSelectionBorder(5f);
            onNodeSelected?.Invoke(this);
        }
        
        ///<summary>
        /// Called when this node view gets unselected.
        ///</summary>
        public override void OnUnselected()
        {
            ShowSelectionBorder(0f);
            BehaviorTreeManager.selectedObject = null;
        }

        ///<summary>
        /// Show or hide node border.
        ///</summary>
        ///<param name="width">the width of node border</param>
        protected void ShowSelectionBorder(float width)
        {
            nodeBorder.style.color = Color.blue;
            nodeBorder.style.borderRightWidth = width;
            nodeBorder.style.borderLeftWidth = width;
            nodeBorder.style.borderTopWidth = width;
            nodeBorder.style.borderBottomWidth = width;
        }
        
        ///<summary>
        /// Set the position of this node view.
        ///</summary>
        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            // Constantly update node position in the graph with
            // Undo/redo support
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
        /// Draw basic node layout
        ///</summary>
        private void Draw()
        {
            // Initialize node ports
            CreateInputPort();
            CreateOutputPort();
            RefreshExpandedState();
        }

        ///<summary>
        /// Show or hide node border.
        ///</summary>
        public void SortChildrenNodes()
        {
            BT_CompositeNode compositeNode = node as BT_CompositeNode;
            if (compositeNode != null)
            {
                compositeNode.childrens.Sort(SortByPosition);
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