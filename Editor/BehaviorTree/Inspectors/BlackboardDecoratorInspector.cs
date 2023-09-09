using System;
using System.Collections;
using System.Collections.Generic;
using BT.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    [CustomEditor(typeof(BlackboardDecorator))]
    public class BlackboardDecoratorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // Start updating the inspected object
            serializedObject.Update();
            
            // Find the blackboard who owns the inspected decorator node.
            Blackboard blackboard = (serializedObject.targetObject as BlackboardDecorator)?.GetBlackboard();
            if (blackboard != null)
            {
                // Update the blackboard decorator key param with the selected option.
                SerializedProperty blackboardKey = serializedObject.FindProperty("blackboardKey");
                
                // If we've found the blackboard, then get an array of all it's variables
                string[] variableNames = blackboard.GetVariablesNames();
                int selectedIndex = Array.IndexOf(variableNames, blackboardKey.stringValue);
                Debug.Log(selectedIndex);
                selectedIndex = EditorGUILayout.Popup("Blackboard Key", selectedIndex, variableNames);
                
                // If selection index is not invalid(e.g. the user has selected an option),
                // update the property.
                if (selectedIndex != -1)
                {
                    blackboardKey.stringValue = variableNames[selectedIndex];
                }
            }
            
            // Apply changes to the inspected object.
            serializedObject.ApplyModifiedProperties();
        }
    }
}

