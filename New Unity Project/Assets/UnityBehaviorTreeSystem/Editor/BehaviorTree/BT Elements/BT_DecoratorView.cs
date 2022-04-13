using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace BT
{

    public class BT_DecoratorView : BT_NodeVisualElement
    {
        
        public delegate void OnDecoratorSelected(BT_DecoratorView decoratorViewSelected);

        private VisualElement decoratorBorder;
        private Label nameLabel;
        private Label descriptionLabel;

        public BT_DecoratorView(BT_NodeView parentView, BT_Node node, string filepath) : base(parentView, node, filepath)
        {
            
        }

        protected override void InitializeUIElements()
        {
            decoratorBorder = contentContainer.Q<VisualElement>("DecoratorBorder");
            nameLabel = contentContainer.Q<Label>("Decorator-Name");
            descriptionLabel = contentContainer.Q<Label>("decorator-description");
            
            nameLabel.bindingPath = "nodeName";
            nameLabel.Bind(new SerializedObject(node));

            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));

            // Register this view as a child for the given node view and add it to the
            // UI Elements hyerarchy.
            parentView.decoratorViews.Add(this);
            parentView.decoratorsContainer.Add(this);
        }

        public override void OnSelected(MouseDownEvent eventData)
        {
            base.OnSelected(eventData);
            ShowSelectionBorder(decoratorBorder, 2f, Color.yellow);
        }
        
        public override void OnUnselected()
        {
           ShowSelectionBorder(decoratorBorder, 2f, Color.black);
           Debug.Log("deselected");
        }
    }
}

