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
        private VisualElement decoratorBorder;
        private Label nameLabel;
        private Label typeNameLabel;
        private Label descriptionLabel;

        private const string DECORATOR_PATH = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ChildNodeViews/Decorator/DecoratorView.uxml";
        
        public BT_DecoratorView(BT_ParentNodeView parentView, BT_ChildNode node, BehaviorTreeGraphView graph) : base(parentView, node, DECORATOR_PATH)
        {
        }
        
        ///<summary>
        /// Initialize UI Elements for this node view.
        ///</summary>
        protected override void InitializeUIElements()
        {
            decoratorBorder = contentContainer.Q<VisualElement>("decorator-border");
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
            // UI Elements hierarchy.
            if(parentView is IParentView view)
                view.AddChildView<BT_DecoratorView>(this);
            
            parentView.decoratorsContainer.Add(this);
        }
        
        public override void OnSelected()
        {
            base.OnSelected();
            selectedCallback.Invoke(this);
            ShowSelectionBorder(decoratorBorder, 5f, Color.yellow);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            selectedCallback.Invoke(this);
            ShowSelectionBorder(decoratorBorder, 2f, Color.black);
        }

        public T GetParentView<T>() where T : BT_ParentNodeView, IChildView
        {
            return parentView as T;
        }
    }
}

