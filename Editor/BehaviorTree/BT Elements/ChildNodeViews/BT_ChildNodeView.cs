using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;
using UnityEditor.Experimental.GraphView;

namespace BT.Editor
{
    ///<summary>
    /// A visual element which can be attached to a BT node
    ///</summary>
    public abstract class BT_ChildNodeView : VisualElement
    {

        public string displayedName;
        public string displayedDescription;
        
        ///<summary>
        /// The parent view of this visual element.
        ///</summary>
        public BT_NodeView parentView;
        
        ///<summary>
        /// The node contained inside this behavior tree visual element.
        ///</summary>
        public BT_ChildNode node;

        ///<summary>
        /// Callback which is called when the decorator view gets selected.
        ///</summary>
        public Action<BT_ChildNodeView> selectedCallback;
        
        ///<summary>
        /// the filepath of the visual element uxml file.
        ///</summary>
        protected string path;

        protected BT_ChildNodeView(BT_NodeView parentView, BT_ChildNode node, string path)
        {
            this.parentView = parentView;
            this.node = node;
            this.path = path;
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(this.path);
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
        /// Called on node view creation and used to initialize UI elements
        /// for this node view.
        ///</summary>
        protected abstract void InitializeUIElements();
        
        ///<summary>
        /// Called when this visual element gets selected.
        ///</summary>
        ///<param name="evt"> Mouse event</param>
        private void OnSelected(MouseDownEvent evt)
        {
            BehaviorTreeManager.selectedObject = this;
            parentView.Unselect(parentView.graph);
            selectedCallback?.Invoke(this);
        }
        
        ///<summary>
        /// Called when this visual element gets unselected.
        ///</summary>
        public abstract void OnUnselected();
        
        ///<summary>
        /// Called when the mouse cursor leaves the visual element.
        ///</summary>
        ///<param name="evt"> Mouse event </param>
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            BehaviorTreeManager.hoverObject = parentView;
        }
        
        ///<summary>
        /// Called when the mouse cursor enters the visual element.
        ///</summary>
        ///<param name="evt"> Mouse event </param>
        protected void OnMouseEnter(MouseEnterEvent evt)
        {
            BehaviorTreeManager.hoverObject = this;
        }
        
        ///<summary>
        /// Modify selection border display.
        ///</summary>
        ///<param name="element"> the element you want to modify it's border </param>
        ///<param name="width"> the width of the border </param>
        ///<param name="borderColor"> the color of the border </param>
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

