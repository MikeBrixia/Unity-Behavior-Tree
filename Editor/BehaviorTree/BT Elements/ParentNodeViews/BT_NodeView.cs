
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    ///<summary>
    /// Base class for node views.
    ///</summary>
    public abstract class BT_NodeView : Node
    {

        ///<summary>
        /// Reference to the node encapsulated inside this node view, this value is going
        /// to contain the actual instructions of the node
        ///</summary>
        public BT_ParentNode node { get; protected set; }
        
        ///<summary>
        /// The parent view at which this node view is connected with
        /// it's input port
        ///</summary>
        public BT_NodeView parentView;
        
        ///<summary>
        /// Called when this node view gets selected by the user
        ///</summary>
        public Action<BT_NodeView> onNodeSelected;
        
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
        /// The position of the mouse.
        ///</summary>
        private Vector2 mousePosition;
        
        ///<summary>
        /// The displayed node name
        ///</summary>
        private Label nodeNameLabel;
        
        /// <summary>
        /// The displayed node type name.
        /// </summary>
        private Label nodeTypeNameLabel;
        
        ///<summary>
        /// The displayed node description
        ///</summary>
        private Label nodeDescriptionLabel;
        private VisualElement titleElement;
        private VisualElement nodeBorder;

        protected BT_NodeView(BT_ParentNode node, BehaviorTreeGraphView graph, string path) : base(path)
        {
            this.viewDataKey = node.guid.ToString();
            this.node = node;
            this.graph = graph;

            InitializeUIElements();
            
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

    }
}

