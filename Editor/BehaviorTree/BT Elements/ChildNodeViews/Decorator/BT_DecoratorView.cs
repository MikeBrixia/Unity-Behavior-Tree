using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;
using UnityEditor.Experimental.GraphView;
using Blackboard = UnityEditor.Experimental.GraphView.Blackboard;

namespace BT.Editor
{
    ///<summary>
    /// Class used to display decorator view.
    ///</summary>
    public class BT_DecoratorView : BT_ChildNodeView
    {
        /// <summary>
        /// The border element of the decorator view.
        /// Usually it is yellow when selected, black otherwise.
        /// </summary>
        private VisualElement decoratorBorder;
        
        /// <summary>
        /// The label on which there is written the specific
        /// user-assigned node name.
        /// </summary>
        private Label nameLabel;
        
        /// <summary>
        /// The label which displays the type of the node.
        /// </summary>
        private Label typeNameLabel;
        
        /// <summary>
        /// Label which displays the description of the node,
        /// used as a comment to explain node functionality.
        /// </summary>
        private Label descriptionLabel;
        
        public BT_DecoratorView(BT_ParentNodeView parentView, BT_ChildNode node, BehaviorTreeGraphView graph) : base(parentView, node, 
            BTInstaller.configSrc + "Editor/BehaviorTree/BT Elements/ChildNodeViews/Decorator/DecoratorView.uxml")
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
            parentView.AddChildView<BT_DecoratorView>(this);
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

        public override T GetParentView<T>()
        {
            return parentView as T;
        }
    }
}

