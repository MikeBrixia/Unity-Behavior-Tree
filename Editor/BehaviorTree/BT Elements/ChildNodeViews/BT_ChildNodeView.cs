using System;
using System.Linq;
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
    public abstract class BT_ChildNodeView : GraphElement
    {
        ///<summary>
        /// The parent view of this visual element.
        ///</summary>
        public readonly BT_ParentNodeView parentView;
        
        ///<summary>
        /// The node contained inside this behavior tree visual element.
        ///</summary>
        public readonly BT_ChildNode node;

        ///<summary>
        /// Callback which is called when the decorator view gets selected.
        ///</summary>
        public Action<BT_ChildNodeView> selectedCallback;

        protected BT_ChildNodeView(BT_ParentNodeView parentView, BT_ChildNode node, string path)
        {
            this.parentView = parentView;
            this.node = node;
            
            // Add child node uxml asset to visual tree.
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            VisualElement childRoot = visualTree.Instantiate();
            
            // Ignore graph element template container.
            Add(childRoot.Children().First());

            // Create child node GUI.
            OnCreateGUI();
        }
        
        private void OnCreateGUI()
        {
            // Remove graph element default stylesheet.
            contentContainer.RemoveFromClassList("graphElement");
            
            InitializeUIElements();
        }

        ///<summary>
        /// Called on node view creation and used to initialize UI elements
        /// for this node view.
        ///</summary>
        protected abstract void InitializeUIElements();
        
        /// <summary>
        /// Get the parent view of this child.
        /// </summary>
        /// <typeparam name="T"> The type of the parent view. </typeparam>
        /// <returns> The parent view. </returns>
        public abstract T GetParentView<T>() where T : BT_ParentNodeView;
        
        public override bool IsSelectable()
        {
            return !selected;
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

