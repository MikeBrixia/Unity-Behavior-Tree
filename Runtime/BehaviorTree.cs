using System;
using System.Collections.Generic;
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

        public void AddNode(BT_Node node)
        {
            
        }

        public void InsertNode(BT_Node node)
        {
            
        }

        public void RemoveNode(BT_Node node)
        {
            
        }
        
        // Behavior tree custom iterator.
        public IEnumerator<BT_Node> GetEnumerator()
        {
            BT_ParentNode currentNode = (BT_ParentNode) rootNode.childNode;
            while (true)
            {
                //TODO implement behavior tree iterator.
                yield return currentNode;
            }
        }
        
        ///<summary>
        /// Clone this behavior tree asset
        ///</summary>
        ///<returns> A copy of this Behavior Tree asset</returns>
        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone() as BT_RootNode;
            tree.blackboard = tree.blackboard.Clone();
            // Initialize behavior tree and blackboard references on each node of the tree
            tree.rootNode.SetBlackboard(tree.blackboard);
            return tree;
        }
    }
}

