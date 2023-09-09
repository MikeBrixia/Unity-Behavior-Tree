using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using BT.Editor;
using BT.Runtime;

namespace BT
{
    public class BehaviorTreeInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeInspector, UxmlTraits> { }
        
        ///<summary>
        /// The editor which is going to inspect the node.
        ///</summary>
        private UnityEditor.Editor inspectorEditor;
        
        ///<summary>
        /// Update the inspector view.
        ///</summary>
        ///<param name="nodeToInspect"> the node to inspect </param>
        public void UpdateInspector(BT_Node nodeToInspect)
        {
            // Clear Inspector view and reference before creating a new editor
            Clear();
            Object.DestroyImmediate(inspectorEditor);
            UnityEditor.Editor.CreateEditor(new Object[] {nodeToInspect});
            
            // Initialize new node inspector editor.
            inspectorEditor = UnityEditor.Editor.CreateEditor(new Object[] {nodeToInspect});
            
            if (inspectorEditor != null)
            {
                inspectorEditor.UseDefaultMargins();
                // Create action to pass as a parameter to the IMGUI Container
                IMGUIContainer container = new IMGUIContainer(() =>
                {
                    if (inspectorEditor.target != null)
                    {
                        inspectorEditor.OnInspectorGUI();
                    }
                });
                Add(container);
            }
        }
    }
}

