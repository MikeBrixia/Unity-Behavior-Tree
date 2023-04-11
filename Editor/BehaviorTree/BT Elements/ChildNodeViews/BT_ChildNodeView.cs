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
        
        protected const string CHILD_NODE_STYLE_PATH =
            "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ChildNodeViews/child-node-container.uss";
        
        protected BT_ChildNodeView(BT_ParentNodeView parentView, BT_ChildNode node, string path)
        {
            this.parentView = parentView;
            this.node = node;
            
            // Add child node uxml asset to visual tree.
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            VisualElement childRoot = visualTree.Instantiate();
            
            StyleSheet asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(CHILD_NODE_STYLE_PATH);
            childRoot.contentContainer.styleSheets.Add(asset);
            
            // Ignore graph element template container.
            Add(childRoot.Children().First());
            
            // Remove graph element default stylesheet.
            contentContainer.RemoveFromClassList("graphElement");
            
            // Create child node GUI.
            OnCreateGUI();
        }
        
        private void OnCreateGUI()
        {
            InitializeUIElements();
        }

        ///<summary>
        /// Called on node view creation and used to initialize UI elements
        /// for this node view.
        ///</summary>
        protected abstract void InitializeUIElements();

        public override bool IsSelectable()
        {
            return !selected;
        }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            Debug.Log("ciao");
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

