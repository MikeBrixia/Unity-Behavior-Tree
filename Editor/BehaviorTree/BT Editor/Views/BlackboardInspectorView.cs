using System;
using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BT
{
    internal sealed class BlackboardInspectorView : IMGUIContainer
    {
        public new class UxmlFactory : UxmlFactory<BlackboardInspectorView, UxmlTraits> { }
        
        /**
         * The inspector editor for the blackboard.
         */
        private BlackboardInspector blackboardInspector;

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
             Add(inspectorGUI); ;
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
             BehaviorTree tree = Selection.activeObject as BehaviorTree;
             if (tree != null)
             {
                 // Build asset path for new blackboard relative to the Behavior Tree asset path.
                 string filepath = AssetDatabase.GetAssetPath(tree);
                 int startIndex = filepath.LastIndexOf("/", StringComparison.Ordinal);
                 filepath = filepath.Remove(startIndex);
                 // The name of the new blackboard asset will be: {BehaviorTreeAssetName}_Blackboard.
                 string newBlackboardFilename = tree.name + "_Blackboard";
                 
                 // The path of the blackboard wll be: {BehaviorTreeAssetPath}/{BehaviorTreeAssetName}_Blackboard.asset
                 string assetPath = filepath + "/" + newBlackboardFilename + ".asset";
                 string[] assets = AssetDatabase.FindAssets(newBlackboardFilename);
                 
                 // Is there another blackboard with the same filename?
                 if (assets.Length > 0)
                 {
                     // If true, then blackboard filename will be: {BehaviorTreeAssetName}_Blackboard_{CopyCount}
                     newBlackboardFilename = newBlackboardFilename + "_" + assets.Length;
                     // And asset path will be: {BehaviorTreeAssetPath}/{BehaviorTreeAssetName}_Blackboard_{CopyCount}.asset
                     assetPath = filepath + "/" + newBlackboardFilename + ".asset";
                 }
                 
                 // Finally, create the asset.
                 Blackboard newBlackboard = ScriptableObject.CreateInstance<Blackboard>();
                 AssetDatabase.CreateAsset(newBlackboard, assetPath);
                 
                 // Set this blackboard as the new tree blackboard and inspect it.
                 tree.SetBlackboard(newBlackboard);
                 InspectBlackboard(newBlackboard);
                 
                 // Once we've finished creating the blackboard asset, save
                 // all assets just for precaution.
                 AssetDatabase.SaveAssets();
             }
         }
    }
}