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
            refreshButton.clicked += AssetDatabase.Refresh;
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
        
        /**
         * Refresh the behavior tree editor and
         * the asset it's currently editing.
         */
        private void RefreshEditorAndAssets()
        {
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
                treeViewLabel.text = " Tree View: " + behaviorTree.name;
                graphView.tree = behaviorTree;
                
                // When the user selects a behavior tree we need to populate
                // the graph view with the tree asset data.
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
