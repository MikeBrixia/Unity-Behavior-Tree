using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    public class BlackboardInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardInspector, UxmlTraits> { }
        
        private UnityEditor.Editor blackboardInspector;

        public void UpdateInspector(BehaviorTree blackboardToInspect)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(blackboardInspector);
            blackboardInspector = UnityEditor.Editor.CreateEditorWithContext(new Object[] { blackboardToInspect }, null, typeof(BehaviorTree));
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (blackboardInspector.target != null)
                {
                    blackboardInspector.OnInspectorGUI();
                }
            });
        }
    }
}

