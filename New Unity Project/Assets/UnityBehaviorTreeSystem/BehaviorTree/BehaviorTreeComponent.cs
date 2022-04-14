using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

namespace BT
{
    public class BehaviorTreeComponent : MonoBehaviour
    {
        ///<summary>
        /// The behavior tree this component is responsible to run
        ///</summary>
        [SerializeField] private BehaviorTree behaviorTree;
        
        [Tooltip("The interval at which the Behavior tree it's going to execute")]
        public float UpdateInterval = 0.1f;

        ///<summary>
        /// The blackboard component used by the current behavior tree
        ///</summary>
        public Blackboard blackboard
        {
            get
            {
                return behaviorTree.blackboard;
            }
        }
        
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
        
        private void ExecuteBehaviorTree()
        {
            behaviorTree.treeState = behaviorTree.rootNode.ExecuteNode();
        }
    }

}
