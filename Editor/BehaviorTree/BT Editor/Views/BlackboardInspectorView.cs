using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT
{
    internal sealed class BlackboardInspectorView : IMGUIContainer
    {
        public new class UxmlFactory : UxmlFactory<BlackboardInspectorView, UxmlTraits> { }
        
        /**
         * The inspector editor for the blackboard.
         */
        private BlackboardInspector blackboardInspector;

        public BlackboardInspectorView()
        {
        }
        
         /// <summary>
         /// Initialize the blackboard inspector view. This operation
         /// will create an inspector editor to inspect the target blackboard
         /// and will add it to this IMGUI container.
         /// </summary>
         /// <param name="blackboard"> The blackboard asset to inspect. </param>
        public void InspectBlackboard(Blackboard blackboard)
         {
             // Clear the previous inspector GUI.
             Clear();
             
             VisualElement inspectorGUI;
             // Is the blackboard valid?
             if (blackboard != null)
             {
                 // If true, then create the blackboard inspector editor with a target object to inspect.
                 blackboardInspector = (BlackboardInspector) UnityEditor.Editor.CreateEditorWithContext(new Object[] {blackboard}, null, typeof(BlackboardInspector));
                 inspectorGUI = blackboardInspector.CreateInspectorGUI();
             }
             else
             {
                 inspectorGUI = CreateInvalidBlackboardGUI();
             }
             
             // Display inspector GUI inside the editor blackboard view
             Add(inspectorGUI);
             // For some reason, saving assets is the only way to make the 
             // scroll view work; Otherwise it could not show up in the view.
             // My only guess is it has something to do with the OnValidate()
             // method which fires the event inside the BehaviorTree asset.
             AssetDatabase.SaveAssets();
         }

         private VisualElement CreateInvalidBlackboardGUI()
         {
             VisualElement invalidBlackboardGUI = new VisualElement()
             {
                  style =
                  {
                      marginTop = 100
                  }
             };
             
             Label invalidBlackboardLabel = CreateInvalidBlackboardLabel();
             Button createBlackboardButton = CreateInvalidBlackboardButton();
             
             invalidBlackboardGUI.Add(invalidBlackboardLabel);
             invalidBlackboardLabel.Add(createBlackboardButton);

             return invalidBlackboardGUI;
         }

         private Label CreateInvalidBlackboardLabel()
         {
             // Initialize label.
             Label invalidBlackboardLabel = new Label("No blackboard asset assigned to this Behavior Tree.")
             {
                 style =
                 {
                     alignItems = Align.Center,
                     unityTextAlign = TextAnchor.MiddleCenter,
                     whiteSpace = WhiteSpace.Normal
                 }
             };
             return invalidBlackboardLabel;
         }

         private Button CreateInvalidBlackboardButton()
         {
             // Initialize create blackboard button.
             Button createBlackboardButton = new Button(OnCreateBlackboard)
             {
                 style =
                 {
                     alignItems = Align.Center,
                     top = 30
                 },
                 text = "Create Blackboard Asset"
             };
             return createBlackboardButton;
         }
         
         private void OnCreateBlackboard()
         {
             Debug.Log("Create blackboard!");
         }
    }
}