using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT
{
    public class BT_ServiceView : BT_NodeVisualElement
    {
        public BT_Service service;
        private VisualElement serviceBorder;
        private Label serviceNameLabel;
        private Label serviceDescriptionLabel;
        private Label serviceUpdateLabel;

        public BT_ServiceView(BT_NodeView parentView, BT_Node node, string filepath) : base(parentView, node, filepath)
        {
        }

        protected override void InitializeUIElements()
        {
            // Get visual tree asset elements
            serviceBorder = contentContainer.Q<VisualElement>("ServiceBorder");
            serviceNameLabel = contentContainer.Q<Label>("ServiceName");
            serviceDescriptionLabel = contentContainer.Q<Label>("ServiceDescription");
            serviceUpdateLabel = contentContainer.Q<Label>("ServiceUpdateFrequencyLabel");
            
            // Initialize view name label
            serviceNameLabel.bindingPath = "nodeName";
            serviceNameLabel.Bind(new SerializedObject(service));

            // Initialize view description label
            serviceDescriptionLabel.bindingPath = "description";
            serviceDescriptionLabel.Bind(new SerializedObject(service));

        }

        protected override void OnSelected(MouseDownEvent evt)
        {
            base.OnSelected(evt);
            ShowSelectionBorder(serviceBorder, 2f, Color.yellow);
        }

        protected override void OnMouseEnter(MouseEnterEvent evt)
        {
            base.OnMouseEnter(evt);
        }

        protected override void OnMouseLeave(MouseLeaveEvent evt)
        {
            base.OnMouseLeave(evt);
        }
    }
}

