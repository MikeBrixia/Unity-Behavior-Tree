using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime
{

    ///<summary>
    /// Behavior Tree asset which contains all the data needed from the BehaviorTreeComponent
    /// to make it run.
    /// Behavior Tree execute from left to right and from top to bottom
    ///</summary>
    [CreateAssetMenu(fileName = "New Behavior Tree", menuName = "AI/Behavior Tree")]
    public sealed class BehaviorTree : ScriptableObject
    {

        ///<summary>
        /// The blackboard attached to the behavior tree.
        ///</summary>
        public Blackboard blackboard;

        ///<summary>
        /// The entry point node of the behavior tree.
        ///</summary>
        [HideInInspector] public BT_RootNode rootNode;

        ///<summary>
        /// The current state of this Behavior Tree.
        ///</summary>
        [HideInInspector] public ENodeState treeState = ENodeState.Waiting;
        
#if UNITY_EDITOR
        ///<summary>
        /// All the Behavior Tree nodes. This is an editor only property
        /// used by the graph to draw all nodes. </summary>
        [HideInInspector] public List<BT_Node> nodes = new List<BT_Node>();
        
        // delegate used to listen for blackboard changes.
        public delegate void OnBlackboardChange(Blackboard blackboard);

        /// <summary>
        /// Called when the user changes or invalidates the tree blackboard.
        /// </summary>
        public OnBlackboardChange onBlackboardChange;
        
        /// <summary>
        /// Unique identifier of the behavior tree asset.
        /// </summary>
        [HideInInspector] public GUID guid;
        
        public void OnEnable()
        {
            guid = GUID.Generate();
        }
        
        public void OnValidate()
        {
            onBlackboardChange?.Invoke(blackboard);
            
            // Update the blackboard on this tree and
            // all it's nodes
            SetBlackboard(blackboard);
        }
        
        /// <summary>
        /// Check if this asset is a clone of the given tree asset.
        /// </summary>
        /// <param name="tree"> The tree from which this asset has been cloned.</param>
        /// <returns> True if this tree is a clone of input tree, false otherwise.</returns>
        public bool IsCloneOf(BehaviorTree tree)
        {
            return guid.Equals(tree.guid);
        }
        
        public void SetBlackboard(Blackboard blackboard)
        {
            this.blackboard = blackboard;
            rootNode.SetBlackboard(blackboard);
        }
#endif

        ///<summary>
        /// Clone this behavior tree asset
        ///</summary>
        ///<returns> A copy of this Behavior Tree asset</returns>
        public BehaviorTree Clone()
        {
            // Clone the behavior tree asset.
            BehaviorTree tree = Instantiate(this);
#if UNITY_EDITOR
            tree.guid = guid;
#endif
            // Clone all the nodes.
            tree.rootNode = tree.rootNode.Clone() as BT_RootNode;
            
            // Clone the blackboard and update it on all
            // behavior tree nodes.
            tree.blackboard = tree.blackboard.Clone();
            tree.rootNode.SetBlackboard(tree.blackboard);
            
            return tree;
        }
    }
}

