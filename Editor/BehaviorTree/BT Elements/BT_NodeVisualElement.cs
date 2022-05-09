using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    ///<summary>
    /// A visual element which can be attached to a BT node
    ///</summary>
    public abstract class BT_NodeVisualElement : VisualElement
    {

        public string displayedName;
        public string displayedDescription;
        
        ///<summary>
        /// The parent view of this visual element
        ///</summary>
        public BT_NodeView parentView;
        
        ///<summary>
        /// The node contained inside this behavior tree visual element
        ///</summary>
        public BT_Node node;

        ///<summary>
        /// Callback which is called when the decorator view gets selected
        ///</summary>
        public Action<BT_NodeVisualElement> selectedCallback;
        
        ///<summary>
        /// the filepath of the visual element uxml
        ///</summary>
        protected string filepath;

        public BT_NodeVisualElement(BT_NodeView parentView, BT_Node node, string filepath)
        {
            this.parentView = parentView;
            this.node = node;
            this.filepath = filepath;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(this.filepath);
            VisualElement decoratorRoot = visualTree.Instantiate();
            Add(decoratorRoot);
            
            InitializeViewInputOutput();
            InitializeUIElements();
        }
        
        ///<summary>
        /// Called on node view creation and used to initialize custom input events for this
        /// visual element
        ///</summary>
        protected virtual void InitializeViewInputOutput()
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
        /// for this node view
        ///</summary>
        protected abstract void InitializeUIElements();

        public virtual void OnSelected(MouseDownEvent evt)
        {
            BehaviorTreeSelectionManager.selectedObject = this;
            parentView.Unselect(parentView.behaviorTreeGraph);
            selectedCallback?.Invoke(this);
        }
        
        public abstract void OnUnselected();

        protected virtual void OnMouseLeave(MouseLeaveEvent evt)
        {
            BehaviorTreeSelectionManager.hoverObject = parentView;
        }

        protected virtual void OnMouseEnter(MouseEnterEvent evt)
        {
            BehaviorTreeSelectionManager.hoverObject = this;
        }

        protected void ShowSelectionBorder(VisualElement element, float width, Color borderColor)
        {
            // Change color
            element.style.borderTopColor = borderColor;
            element.style.borderBottomColor = borderColor;
            element.style.borderRightColor = borderColor;
            element.style.borderLeftColor = borderColor;
            
            // Change width
            element.style.borderRightWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderTopWidth = width;
            element.style.borderBottomWidth = width;
        }
    }
}

