using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;
using UnityEditor.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    [SerializeField]
    public class BehaviorTreeEditor : EditorWindow
    {
        private BehaviorTree behaviorTree;
        private BehaviorTreeGraphView graphView;
        private NodeInspectorView nodeInspectorView;
        private BlackboardInspectorView blackboardInspectorView;
        private Label treeViewLabel;
        private ToolbarButton saveButton;
        private ToolbarButton refreshButton;
        private SerializedObject serializedBlackboard;

        ///<summary>
        /// Open the behavior tree editor window.
        ///</summary>
        [MenuItem("Window/AI/Behavior Tree Editor")]
        public static void OpenWindow()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("Behavior Tree Editor");
        }
        
        ///<summary>
        /// Open the behavior tree editor.
        ///</summary>
        [OnOpenAsset]
        public static bool OpenEditor(int instanceID, int line)
        {
            bool canOpen = Selection.activeObject is BehaviorTree;
            if (canOpen)
            {
                OpenWindow();
            }
            return canOpen;
        }
        
        ///<summary>
        /// Create behavior tree editor GUI.
        ///</summary>
        public void CreateGUI()
        {
            // The currently selected behavior tree.
            behaviorTree = Selection.activeObject as BehaviorTree;
            
            // Load behavior tree UXML file and make a copy of it.
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Editor/BehaviorTreeEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
            
            // Initialize all the behavior tree editor views.
            graphView = rootVisualElement.Q<BehaviorTreeGraphView>();
            
            saveButton = rootVisualElement.Q<ToolbarButton>("SaveButton");
            refreshButton = rootVisualElement.Q<ToolbarButton>("RefreshButton");
            
            // Initialize toolbar buttons click events.
            saveButton.clicked += AssetDatabase.SaveAssets;
            refreshButton.clicked += RefreshEditorAndAssets;
            
            // Handle blackboard inspector GUI events.
            blackboardInspectorView = rootVisualElement.Q<BlackboardInspectorView>("BlackboardInspector");
            nodeInspectorView = rootVisualElement.Q<NodeInspectorView>();
            if (behaviorTree != null)
            {
                behaviorTree.onBlackboardChange += blackboard => blackboardInspectorView.InspectBlackboard(blackboard);
                blackboardInspectorView.InspectBlackboard(behaviorTree.blackboard);
                nodeInspectorView.InspectNode(behaviorTree.rootNode);
            }
            
            treeViewLabel = rootVisualElement.Q<Label>("Tree_View_Label");

            // Initialize Callback for when the node selection changes from a node to another node
            graphView.onNodeSelected = OnNodeSelectionChange;
            graphView.onChildNodeSelected = OnNodeVisualElementSelectionChange;

            OnSelectionChange();
        }
        
        /// <summary>
        /// Refresh command for the behavior tree editor.
        /// Use it when you want to ensure the data consistency
        /// of the editor.
        /// Users can also call this command from the toolbar
        /// when some issues occurs.
        /// </summary>
        private void RefreshEditorAndAssets()
        {
            if (behaviorTree != null)
            {
                // Make sure the blackboard is not a missing value or reference.
                ValidateBlackboard(behaviorTree.blackboard);
            }
            
            // Finally, perform an asset database full refresh.
            AssetDatabase.Refresh();
        }
        
        private void ValidateBlackboard(Blackboard blackboard)
        {
            bool isMissing = !ReferenceEquals(blackboard, null) && (!blackboard);
            // Is the blackboard value missing(not null but also not valid)?
            if (isMissing)
            {
                // If it is missing, then set the blackboard as null
                // in all tree nodes.
                behaviorTree.SetBlackboard(null);
                // and finally update the blackboard inspector view
                blackboardInspectorView.InspectBlackboard(null);
            }
        }
        
        ///<summary>
        /// Called when the behavior tree editor selection change.
        ///</summary>
        private void OnSelectionChange()
        {
            // The currently selected behavior tree.
            behaviorTree = Selection.activeObject as BehaviorTree;
            if (behaviorTree != null)
            {
                // Update the editor tree label depending on the selected tree.
                treeViewLabel.text = " Tree View: " + behaviorTree.name;
                graphView.tree = behaviorTree;
                
                // When the user selects a behavior tree we need to populate
                // the graph view with the tree nodes.
                graphView.PopulateView();
            }
        }

        ///<summary>
        /// Called when the behavior tree editor selected node changes.
        ///</summary>
        private void OnNodeSelectionChange(BT_ParentNodeView parentNodeView)
        {
            BehaviorTreeManager.selectedObject = parentNodeView;
            if (nodeInspectorView != null)
            {
                nodeInspectorView.InspectNode(parentNodeView.node);
            }
        }
        
        ///<summary>
        /// Called when node visual element changes.
        ///</summary>
        private void OnNodeVisualElementSelectionChange(BT_ChildNodeView childNodeView)
        {
            BehaviorTreeManager.selectedObject = childNodeView;
            if (nodeInspectorView != null)
            {
                nodeInspectorView.InspectNode(childNodeView.node);
            }
        }
    }
}
