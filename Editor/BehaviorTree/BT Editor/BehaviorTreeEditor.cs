using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using BT.Runtime;

namespace BT.Editor
{
    /// <summary>
    /// The Behavior Tree Editor is the class responsible for
    /// coordinating all the different components of the
    /// user editor, like graphs, inspectors, toolbar, selections ecc...
    /// </summary>
    public class BehaviorTreeEditor : EditorWindow
    {
        /// <summary>
        /// The tree currently being edited by this editor.
        /// </summary>
        public BehaviorTree behaviorTree { protected set; get; }
        
        /// <summary>
        /// The graph view used to interact with tree nodes.
        /// </summary>
        public BehaviorTreeGraphView graphView { protected set; get; }
        
        /// <summary>
        /// The editor debugger, used to debug the behavior tree editor.
        /// </summary>
        private BehaviorTreeDebugger debugger;
        
        /// <summary>
        /// The inspector view used to inspect graph nodes.
        /// </summary>
        private NodeInspectorView nodeInspectorView;
        
        /// <summary>
        /// The inspector view used to inspect the assigned blackboard.
        /// </summary>
        private BlackboardInspectorView blackboardInspectorView;
        
        /// <summary>
        /// The label displaying the name of the currently edited tree.
        /// </summary>
        private Label treeViewLabel;
        
        /// <summary>
        /// Button which users can press when they want to save the edited asset.
        /// </summary>
        private ToolbarButton saveButton;
        
        /// <summary>
        /// Button which users can press when they want to restore data or fix some
        /// unknown issues. 
        /// </summary>
        private ToolbarButton refreshButton;
        
        /// <summary>
        /// The save cache is where the behavior tree editor
        /// temporarily stores saved assets, after the user has pressed the
        /// save button. It can be used to keep track of the
        /// latest saved version of a tree in case there was
        /// a fatal error which caused irreversible data corruption.
        /// </summary>
        private readonly Queue<BehaviorTree> saveCache = new Queue<BehaviorTree>();
        
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
        
        private void OnEnable()
        {
            // The currently selected behavior tree.
            behaviorTree = Selection.activeObject as BehaviorTree;
            
            // If there's not debugger, create one.
            if (debugger == null)
            {
                debugger = new BehaviorTreeDebugger(this);
            }
            
            // Listen for play mode callbacks.
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        private void Update()
        {
            // Updated when user is in editor play mode.
            if (EditorApplication.isPlaying)
            {
                DebugUpdate();
            }
        }
        
        private void DebugUpdate()
        {
            // Find the tree popup selection element inside the toolbar.
            Toolbar toolbar = rootVisualElement.Q<Toolbar>();
            PopupField<BehaviorTreeComponent> instancePopupSelector = (PopupField<BehaviorTreeComponent>) toolbar.ElementAt(2);

            // Debug the selected behavior tree asset.
            BehaviorTreeComponent selectedComponent = instancePopupSelector.value;
            debugger.DebugGraphEditor(selectedComponent.tree);
        }
         
        private void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                // Release the tree from the debug state.
                debugger.ResetDebugEditor();
            }
            
        }

        /// <summary>
        /// Save command for saving edited behavior tree assets and
        /// pushing the to the save cache. This event will also trigger
        /// a full asset database save as a precaution step.
        /// </summary>
        private void SaveAsset()
        {
            // Push the behavior tree to the save cache.
            saveCache.Enqueue(behaviorTree);
            
            // Finally, save all the asset database for precaution.
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Refresh command for the behavior tree editor.
        /// Use it when you want to ensure the data consistency
        /// of the editor.
        /// Users can also call this command from the toolbar.
        /// </summary>
        private void RefreshEditorAndAsset()
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
        /// Create behavior tree editor GUI.
        ///</summary>
        public void CreateGUI()
        {
            // Load behavior tree UXML file and make a copy of it.
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Editor/BehaviorTreeEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
            
            // Initialize graph view.
            graphView = rootVisualElement.Q<BehaviorTreeGraphView>();
            graphView.tree = behaviorTree;
            graphView.onNodeSelected = OnNodeSelectionChange;
            graphView.onChildNodeSelected = OnNodeVisualElementSelectionChange;
            
            CreateToolbarGUI();
            CreateInspectorGUI();
            
            treeViewLabel = rootVisualElement.Q<Label>("Tree_View_Label");
            
            // Trigger a selection event.
            OnSelectionChange();
        }
        
        private void CreateToolbarGUI()
        {
            // Initialize toolbar UI elements.
            saveButton = rootVisualElement.Q<ToolbarButton>("SaveButton");
            refreshButton = rootVisualElement.Q<ToolbarButton>("RefreshButton");
            
            // Initialize toolbar events.
            saveButton.clicked += SaveAsset;
            refreshButton.clicked += RefreshEditorAndAsset;
            
            // All the tree components currently loaded inside the game scene.
            BehaviorTreeComponent[] treeComponents = FindObjectsOfType<BehaviorTreeComponent>();
            
            Toolbar toolbar = rootVisualElement.Q<Toolbar>();
            PopupField<BehaviorTreeComponent> loadedComponents = new PopupField<BehaviorTreeComponent>("Target GameObject:", new List<BehaviorTreeComponent>(treeComponents), treeComponents[0]);
            toolbar.Add(loadedComponents);
        }

        private void CreateInspectorGUI()
        {
            // Handle blackboard inspector GUI events.
            blackboardInspectorView = rootVisualElement.Q<BlackboardInspectorView>("BlackboardInspector");
            nodeInspectorView = rootVisualElement.Q<NodeInspectorView>();
            if (behaviorTree != null)
            {
                if (blackboardInspectorView != null)
                {
                    behaviorTree.onBlackboardChange += blackboard => blackboardInspectorView.InspectBlackboard(blackboard);
                    blackboardInspectorView.InspectBlackboard(behaviorTree.blackboard);
                }

                if (nodeInspectorView != null)
                {
                    nodeInspectorView.InspectNode(behaviorTree.rootNode);
                }
            }
        }
        
        private void OnGUI()
        {
            // Find the tree popup selection element inside the toolbar.
            Toolbar toolbar = rootVisualElement.Q<Toolbar>();
            if (toolbar != null)
            {
                PopupField<BehaviorTreeComponent> instancePopupSelector = (PopupField<BehaviorTreeComponent>) toolbar.ElementAt(2);
            
                // Enable instance selector.
                bool isPlaying = EditorApplication.isPlaying;
                instancePopupSelector.visible = isPlaying;
                instancePopupSelector.SetEnabled(isPlaying);
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
