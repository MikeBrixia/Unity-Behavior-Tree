
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    ///<summary>
    /// Class used to display decorator view.
    ///</summary>
    public class BT_DecoratorView : BT_ChildNodeView, IChildView
    {
        
        public delegate void OnDecoratorSelected(BT_DecoratorView decoratorViewSelected);
        
        private VisualElement decoratorBorder;
        private Label nameLabel;
        private Label typeNameLabel;
        private Label descriptionLabel;
  
        public BT_DecoratorView(BehaviorTreeGraphView graph, BT_NodeView parentView, BT_Node node) : base(parentView, node)
        {
            this.parentView = parentView;
            this.node = node;
            this.visualTreePath = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/DecoratorView.uxml";
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            VisualElement decoratorRoot = visualTree.Instantiate();
            Add(decoratorRoot);
            
            InitializeViewInputOutput();
            InitializeUIElements();
        }
        
        ///<summary>
        /// Called on node view creation and used to initialize custom input events for this
        /// visual element.
        ///</summary>
        private void InitializeViewInputOutput()
        {
            // Register mouse callbacks
            EventCallback<MouseEnterEvent> mouseEnterEvent = OnMouseEnter;
            RegisterCallback<MouseEnterEvent>(mouseEnterEvent);
            EventCallback<MouseLeaveEvent> mouseLeaveEvent = OnMouseLeave;
            RegisterCallback<MouseLeaveEvent>(mouseLeaveEvent);
            EventCallback<MouseDownEvent> mousePressedEvent = OnSelected;
            RegisterCallback<MouseDownEvent>(mousePressedEvent); 
        }
        
        ///<summary>
        /// Initialize UI Elements for this node view.
        ///</summary>
        protected override void InitializeUIElements()
        {
            decoratorBorder = contentContainer.Q<VisualElement>("DecoratorBorder");
            nameLabel = contentContainer.Q<Label>("Decorator-Name");
            typeNameLabel = contentContainer.Q<Label>("Decorator-Type-Name");
            descriptionLabel = contentContainer.Q<Label>("decorator-description");
            
            // Serialized representation of the contained node
            SerializedObject serializedNode = new SerializedObject(node);
            
            nameLabel.bindingPath = "nodeName";
            nameLabel.Bind(serializedNode);

            typeNameLabel.bindingPath = "nodeTypeName";
            typeNameLabel.Bind(serializedNode);
            
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(serializedNode);

            // Register this view as a child for the given node view and add it to the
            // UI Elements hyerarchy.
            if(parentView is IParentView view)
                view.AddChildView<BT_DecoratorView>(this);
            parentView.decoratorsContainer.Add(this);
        }
        
        ///<summary>
        /// Called when this node gets selected.
        ///</summary>
        public void OnSelected(MouseDownEvent eventData)
        {
            ShowSelectionBorder(decoratorBorder, 2f, Color.yellow);
        }
        
        ///<summary>
        /// Called when this node gets unselected.
        ///</summary>
        public override void OnUnselected()
        {
           ShowSelectionBorder(decoratorBorder, 2f, Color.black);
        }
        
        ///<summary>
        /// Called when the mouse cursor leaves the visual element.
        ///</summary>
        ///<param name="evt"> Mouse event </param>
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            BehaviorTreeManager.hoverObject = parentView;
        }
        
        public T GetParentView<T>() where T : BT_NodeView, IChildView
        {
            return parentView as T;
        }
    }
}

