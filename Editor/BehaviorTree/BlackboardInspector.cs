using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    public class BlackboardInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardInspector, UxmlTraits> { }
        
        ///<summary>
        /// The blackboard inspector which is going to inspect the blackboard
        ///</summary>
        private UnityEditor.Editor blackboardInspector;
        
        ///<summary>
        /// Update inspector view
        ///</summary>
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

