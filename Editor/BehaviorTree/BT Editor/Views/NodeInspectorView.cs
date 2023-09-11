using UnityEngine;
using UnityEngine.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    /// <summary>
    /// Class responsible for displaying selected node inspector
    /// inside the behavior tree editor nodes inspector window.
    /// </summary>
    public sealed class NodeInspectorView : IMGUIContainer
    {
        public new class UxmlFactory : UxmlFactory<NodeInspectorView, UxmlTraits> { }
        
        ///<summary>
        /// The editor which is going to inspect the node.
        ///</summary>
        private UnityEditor.Editor nodeInspector;
        
        public void InspectNode(BT_Node node)
        {
            // Remove any previous UI elements to make room for new ones.
            Clear();
            
            VisualElement inspectorGUI;
            // Is the inspected node valid?
            if (node != null)
            {
                // If true, then create the node inspector editor with a target node to inspect.
                this.nodeInspector = UnityEditor.Editor.CreateEditorWithContext(new Object[]{node}, null, typeof(BlackboardInspector));
                inspectorGUI = nodeInspector.CreateInspectorGUI();
            }
            else
            {
                // Otherwise, create and invalid node GUI replacement.
                inspectorGUI = CreateInvalidNodeGUI();
            }
            
            // Display the correct inspector GUI.
            Add(inspectorGUI);
        }

        private VisualElement CreateInvalidNodeGUI()
        {
            Label invalidNodeLabel = new Label("No nodes selected")
            {
                  style =
                  {
                      alignItems = Align.Center,
                      top = 100
                  }
            };

            return invalidNodeLabel;
        }
    }
}

