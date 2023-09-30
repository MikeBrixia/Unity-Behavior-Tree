using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// The component responsible of running the behavior tree.
    ///</summary>
    public class BehaviorTreeComponent : MonoBehaviour
    {
        ///<summary>
        /// The behavior tree this component is responsible to run
        ///</summary>
        [SerializeField] private BehaviorTree behaviorTree;
        
        ///<summary>
        /// The blackboard component used by currently assigned Behavior Tree
        ///</summary>
        public Blackboard blackboard
        {
            get
            {
                return behaviorTree.blackboard;
            }
        }
        
        ///<summary>
        /// The state of the currently running Behavior Tree
        ///</summary>
        public EBehaviorTreeState behaviorTreeState
        {
            get
            {
                return behaviorTree.treeState;
            }
        }
        
        void Awake()
        {
            // Clone behavior tree
            behaviorTree = behaviorTree.Clone();
            behaviorTree.treeState = EBehaviorTreeState.Waiting;
        }

        // Start is called before the first frame update
        void Start()
        {
            RunBehaviorTree(behaviorTree);
        }

        // Update is called once per frame
        void Update()
        {
            if(behaviorTree.canTick)
            {
                ExecuteBehaviorTree();
            }
        }
        
        ///<summary>
        /// Start running a behavior tree
        ///</summary>
        ///<param name="behaviorTree"> The behavior tree you want to run.</param>
        public void RunBehaviorTree(BehaviorTree behaviorTree)
        {
            if(behaviorTree != null)
            {
                if(behaviorTree != this.behaviorTree)
                {
                    this.behaviorTree = behaviorTree.Clone();
                }

                if(!this.behaviorTree.canTick)
                {
                    // When there is a new behavior tree cancel all the updates to the previous tree
                    // and clone the new tree
                    CancelInvoke("ExecuteBehaviorTree");
                    InvokeRepeating("ExecuteBehaviorTree", 0f, this.behaviorTree.updateInterval);
                }
            }
        }
        
        ///<summary>
        /// Execute behavior tree root node
        ///</summary>
        private void ExecuteBehaviorTree()
        {
            behaviorTree.treeState = behaviorTree.rootNode.ExecuteNode();
        }
    }

}
