using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    ///<summary>
    /// Class used to display decorator view.
    ///</summary>
    public class BT_DecoratorView : BT_NodeVisualElement
    {
        public delegate void OnDecoratorSelected(BT_DecoratorView decoratorViewSelected);
        
        private VisualElement decoratorBorder;
        private Label nameLabel;
        private Label typeNameLabel;
        private Label descriptionLabel;
  
        public BT_DecoratorView(BT_NodeView parentView, BT_Node node, string filepath) : base(parentView, node, filepath)
        {
            
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
            parentView.decoratorViews.Add(this);
            parentView.decoratorsContainer.Add(this);
        }
        
        ///<summary>
        /// Called when this node gets selected.
        ///</summary>
        public override void OnSelected(MouseDownEvent eventData)
        {
            base.OnSelected(eventData);
            ShowSelectionBorder(decoratorBorder, 2f, Color.yellow);
        }
        
        ///<summary>
        /// Called when this node gets unselected.
        ///</summary>
        public override void OnUnselected()
        {
           ShowSelectionBorder(decoratorBorder, 2f, Color.black);
        }
    }
}

