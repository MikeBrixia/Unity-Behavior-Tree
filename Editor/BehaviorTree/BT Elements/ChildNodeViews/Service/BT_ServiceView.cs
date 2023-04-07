
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
    public class BT_ServiceView : BT_ChildNodeView, IChildView
    {
        private VisualElement serviceBorder;
        private Label serviceNameLabel;
        private Label serviceTypeNameLabel;
        private Label serviceFrequencyLabel;
        private Label serviceDescriptionLabel;
        private Label serviceUpdateLabel;
        private const string SERVICE_PATH = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ChildNodeViews/Service/ServiceView.uxml";
        
        public BT_ServiceView(BT_ParentNodeView parentView, BT_ChildNode node, BehaviorTreeGraphView graph) : base(parentView, node, SERVICE_PATH)
        {
            InitializeUIElements();
            InitializeViewInputOutput();
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
        /// Called on node view creation and used to initialize UI elements
        /// for this node view.
        ///</summary>
        protected override void InitializeUIElements()
        {
            // Get visual tree asset elements
            serviceBorder = contentContainer.Q<VisualElement>("ServiceBorder");
            serviceNameLabel = contentContainer.Q<Label>("ServiceName");
            serviceTypeNameLabel =  contentContainer.Q<Label>("ServiceTypeName");
            serviceFrequencyLabel = contentContainer.Q<Label>("ServiceUpdateFrequencyLabel");
            serviceDescriptionLabel = contentContainer.Q<Label>("ServiceDescription");
            serviceUpdateLabel = contentContainer.Q<Label>("ServiceUpdateFrequencyLabel");

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
            
            if(parentView is IParentView view)
                view.AddChildView<BT_ServiceView>(this);
            parentView.serviceContainer.Add(this);
        }
        
        ///<summary>
        /// Called when this service view it's selected
        ///</summary>
        public void OnSelected(MouseDownEvent evt)
        {
            ShowSelectionBorder(serviceBorder, 2f, Color.yellow);
        }
        
        ///<summary>
        /// Called when this service view it's unselected
        ///</summary>
        public override void OnUnselected()
        {
           ShowSelectionBorder(serviceBorder, 2f, Color.black);
        }

        ///<summary>
        /// Called when the mouse cursor leaves the visual element.
        ///</summary>
        ///<param name="evt"> Mouse event </param>
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            BehaviorTreeManager.hoverObject = parentView;
        }
        
        
        public T GetParentView<T>() where T : BT_ParentNodeView, IChildView
        {
            return parentView as T;
        }
    }
}

