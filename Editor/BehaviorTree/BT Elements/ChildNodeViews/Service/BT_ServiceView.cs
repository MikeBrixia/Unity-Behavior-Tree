
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    ///<summary>
    /// Class used to display service views.
    ///</summary>
    public sealed class BT_ServiceView : BT_ChildNodeView
    {
        private VisualElement serviceBorder;
        private Label serviceNameLabel;
        private Label serviceTypeNameLabel;
        private Label serviceFrequencyLabel;
        private Label serviceDescriptionLabel;
        private const string SERVICE_PATH = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ChildNodeViews/Service/ServiceView.uxml";
        
        public BT_ServiceView(BT_ParentNodeView parentView, BT_ChildNode node, BehaviorTreeGraphView graph) : base(parentView, node, SERVICE_PATH)
        {
        }
        
        ///<summary>
        /// Called on node view creation and used to initialize UI elements
        /// for this node view.
        ///</summary>
        protected override void InitializeUIElements()
        {
            // Get visual tree asset elements
            serviceBorder = contentContainer.Q<VisualElement>("selection-border");
            serviceNameLabel = contentContainer.Q<Label>("ServiceName");
            serviceTypeNameLabel =  contentContainer.Q<Label>("ServiceTypeName");
            serviceFrequencyLabel = contentContainer.Q<Label>("ServiceUpdateFrequencyLabel");
            serviceDescriptionLabel = contentContainer.Q<Label>("ServiceDescription");

            SerializedObject serializedNode = new SerializedObject(node);
            
            // Initialize frequency label
            serviceFrequencyLabel.bindingPath = "frequencyDescription";
            serviceFrequencyLabel.Bind(serializedNode);

            // Initialize name label
            serviceNameLabel.bindingPath = "nodeName";
            serviceNameLabel.Bind(serializedNode);
            
            // Initialize type name label.
            serviceTypeNameLabel.bindingPath = "nodeTypeName";
            serviceTypeNameLabel.Bind(serializedNode);
            
            // Initialize description label
            serviceDescriptionLabel.bindingPath = "description";
            serviceDescriptionLabel.Bind(serializedNode);
            
            // Register this view as a child for the given node view and add it to the
            // UI Elements hierarchy.
            parentView.AddChildView<BT_ServiceView>(this);
            
        }
        
        public override void OnSelected()
        {
            base.OnSelected();
            selectedCallback.Invoke(this);
            ShowSelectionBorder(serviceBorder, 5f, Color.yellow);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            selectedCallback.Invoke(this);
            ShowSelectionBorder(serviceBorder, 2f, Color.black);
        }

        public override T GetParentView<T>()
        {
            return parentView as T;
        }
    }
}

