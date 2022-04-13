using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BT
{
     
    public class BehaviorTreeInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeInspector, UxmlTraits> { }
        
        private NodeInspector InspectorEditor;
        
        public BehaviorTreeInspector()
        {
        }

        public void UpdateInspector(BT_Node nodeToInspect)
        {
            // Clear Inspector view and reference before creating a new editor
            Clear();
            UnityEngine.Object.DestroyImmediate(InspectorEditor);
            // Initialize new editor
            InspectorEditor = Editor.CreateEditorWithContext(new Object[] { nodeToInspect }, null, typeof(NodeInspector)) as NodeInspector;
            InspectorEditor.UseDefaultMargins(); 
            // Create action to pass as a parameter to the IMGUI Container
            IMGUIContainer Container = new IMGUIContainer(() => 
            { 
                if(InspectorEditor.target != null)
                {
                    InspectorEditor.OnInspectorGUI();
                } 
            });
            Add(Container);
        }
    }
}

