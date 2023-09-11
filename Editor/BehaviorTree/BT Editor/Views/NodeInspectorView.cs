using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using BT.Editor;
using BT.Runtime;

namespace BT.Editor
{
    public class NodeInspectorView : IMGUIContainer
    {
        public new class UxmlFactory : UxmlFactory<NodeInspectorView, UxmlTraits> { }
        
        ///<summary>
        /// The editor which is going to inspect the node.
        ///</summary>
        private UnityEditor.Editor nodeInspector;
        
        public void InspectNode(BT_Node node)
        {
            if (node != null)
            {
                // Create the blackboard inspector editor with a target object to inspect.
                this.nodeInspector = UnityEditor.Editor.CreateEditorWithContext(new Object[]{node}, null, typeof(BlackboardInspector));
                onGUIHandler = nodeInspector.OnInspectorGUI;
            }
        }
    }
}

