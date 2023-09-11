
using UnityEditor;
using BT.Runtime;

namespace BT.Editor
{

    ///<summary>
    /// Custom inspector which handles nodes value changes and reflect them in
    /// to the displayed node
    ///</summary>
    [CustomEditor(typeof(BT_Node), true)]
    public class NodeInspector : UnityEditor.Editor
    {
        private delegate void onValueChange<in TPropertyType>(TPropertyType propertyChanged);
        
        private onValueChange<string> onNodeNameChange;
        private onValueChange<string> onNodeDescriptionChange;
        
        public override void OnInspectorGUI()
        {
            // Load properties
            SerializedProperty serializedNodeName = serializedObject.FindProperty("nodeName");
            string previousNodeName = serializedNodeName.stringValue;
            SerializedProperty serializedNodeDescription = serializedObject.FindProperty("description");
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

