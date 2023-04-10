using System.Collections;
using System.Collections.Generic;
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
        private NodeInspector inspectorEditor;
        
        ///<summary>
        /// Update the inspector view.
        ///</summary>
        ///<param name="nodeToInspect"> the node to inspect </param>
        public void UpdateInspector(BT_Node nodeToInspect)
        {
            // Clear Inspector view and reference before creating a new editor
            Clear();
            Object.DestroyImmediate(inspectorEditor);
            // Initialize new editor
            inspectorEditor = UnityEditor.Editor.CreateEditorWithContext(new Object[] { nodeToInspect }, null, typeof(NodeInspector)) 
                              as NodeInspector;
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

