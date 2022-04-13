using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BT
{

    ///<summary>
    /// Custom inspector which handles nodes value changes and reflect them in
    /// to the displayed node
    ///</summary>
    [CustomEditor(typeof(BT_Node))]
    public class NodeInspector : Editor
    {
        
        private SerializedProperty serializedNodeName;
        private SerializedProperty serializedNodeDescription;

        public delegate void OnValueChange<PropertyType>(PropertyType PropertyChanged);

        public OnValueChange<string> onNodeNameChange;
        public OnValueChange<string> onNodeDescriptionChange;

        public NodeInspector()
        {
            
        }
        
        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            // Load properties
            serializedNodeName = serializedObject.FindProperty("nodeName");
            string previousNodeName = serializedNodeName.stringValue;
            serializedNodeDescription = serializedObject.FindProperty("description");
            string previousDescription = serializedNodeDescription.stringValue;
            
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            // Update node name
            if(previousNodeName != serializedNodeName.stringValue 
               && onNodeNameChange != null)
            {
                onNodeNameChange.Invoke(serializedNodeName.stringValue);
            }
            
            // Update node description
            if(previousDescription != serializedNodeDescription.stringValue 
               && onNodeDescriptionChange != null)
            {
                onNodeDescriptionChange.Invoke(serializedNodeDescription.stringValue);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}

