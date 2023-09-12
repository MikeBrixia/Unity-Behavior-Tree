using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// Behavior Tree possible states.
    ///</summary>
    public enum EBehaviorTreeState { Running, Success, Failed, Waiting }
    
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
        /// if true, the behavior tree is going to update each frame, otherwise
        /// it will use a user defined update interval(updateInterval).
        ///</summary>
        public bool canTick;

        ///<summary>
        /// The rate at which the behavior tree it's going
        /// to be updated. If canTick is set to true this value will
        /// be ignored.
        ///</summary>
        public float updateInterval = 0.1f;
        
        ///<summary>
        /// The current state of this Behavior Tree.
        ///</summary>
        [HideInInspector] public EBehaviorTreeState treeState;
        
        ///<summary>
        /// All the Behavior Tree nodes
        ///</summary>
        [HideInInspector] public List<BT_Node> nodes = new List<BT_Node>();

#if UNITY_EDITOR
        // delegate used to listen for blackboard changes.
        public delegate void OnBlackboardChange(Blackboard blackboard);
        
        /// <summary>
        /// Called when the user changes or invalidates the tree blackboard.
        /// </summary>
        public OnBlackboardChange onBlackboardChange;

        /// <summary>
        /// Unique identifier of the behavior tree asset.
        /// </summary>
        public GUID guid;

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
        
#endif

        public void SetBlackboard(Blackboard blackboard)
        {
            this.blackboard = blackboard;
            foreach (BT_Node node in nodes)
            {
                node.SetBlackboard(blackboard);
            }
        }
        
        ///<summary>
        /// Clone this behavior tree asset
        ///</summary>
        ///<returns> A copy of this Behavior Tree asset</returns>
        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.guid = guid;
            tree.rootNode = tree.rootNode.Clone() as BT_RootNode;
            tree.blackboard = tree.blackboard.Clone();
            // Initialize behavior tree and blackboard references on each node of the tree
            if (tree.rootNode != null)
            {
                tree.rootNode.SetBlackboard(tree.blackboard);
            }
                
            return tree;
        }
    }
}

