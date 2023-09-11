using System;
using System.Collections;
using System.Collections.Generic;
using BT.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    [CustomPropertyDrawer(typeof(BlackboardKeySelector))]
    public class BlackboardKeySelectorPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Toggles key selector property visibility
        /// inside the inspector.
        /// </summary>
        private bool isVisible = true;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Initialize type constrain property.
            SerializedProperty typeConstrain = property.FindPropertyRelative("typeFilter");
            SerializedProperty blackboardKey = property.FindPropertyRelative("blackboardKey");
            
            // Try finding the BT node which has this key as it's member variable.
            BT_Node propertyOwner = property.serializedObject.targetObject as BT_Node;
             
            int selectedIndex = -1;
            string[] variableNames = null;
            
            // Is the key selector a member variable of a BT node?
            if (propertyOwner != null)
            {
                // If true, then find the blackboard who owns the inspected decorator node.
                Blackboard blackboard =  propertyOwner.GetBlackboard();
                
                if (blackboard != null)
                {
                    BlackboardSupportedTypes constrain = (BlackboardSupportedTypes) typeConstrain.enumValueFlag;
                    // If type constrain is "None", allow the user to select any key from the blackboard, otherwise
                    // show him only keys of the constrained type.
                    variableNames = constrain ==  BlackboardSupportedTypes.None? 
                                    blackboard.GetVariablesNames() : blackboard.GetVariableNamesOfType(constrain);
                    selectedIndex = Array.IndexOf(variableNames, blackboardKey.stringValue);
                    
                    // Draw property editor layout elements.
                    isVisible = EditorGUILayout.Foldout(isVisible, "Key selector", true);
                    if (isVisible)
                    {
                        selectedIndex = EditorGUILayout.Popup("Blackboard Key", selectedIndex, variableNames);
                        // If selection index is not invalid(e.g. the user has selected an option),
                        // update the property.
                        blackboardKey.stringValue = selectedIndex != -1 ? variableNames?[selectedIndex] : "None";
                        // If user want to edit advanced settings, this section will be shown to him.
                        EditorGUILayout.PropertyField(typeConstrain);
                    }
                }
            }
        }
    }
}

