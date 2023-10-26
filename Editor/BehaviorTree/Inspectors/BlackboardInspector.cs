
using UnityEditor;
using BT.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Initialize blackboard inspector root.
            VisualElement root = new VisualElement();

            // Calling bind will force all visual element children
            // property fields to create a binding to their respective
            // inspector property. Not calling this function will lead
            // to property fields not showing inside the inspector.
            root.Bind(serializedObject);
            
            // Create inspector visual element from serialized blackboard and add it
            // to the root visual element.
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            return root;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

