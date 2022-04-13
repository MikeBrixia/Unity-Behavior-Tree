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
        
        ///<summary>
        /// Callback for when the behavior tree receives an update
        ///</summary>
        public OnBehaviorTreeUpdate onBehaviorTreeUpdate
        {
            get
            {
                return behaviorTree.onTreeUpdate;
            }
        }

        void Awake()
        {
            // Clone behavior tree
            behaviorTree = behaviorTree.Clone();
        }

        // Start is called before the first frame update
        void Start()
        {
            RunBehaviorTree(behaviorTree);
        }

        // Update is called once per frame
        void Update()
        {
            behaviorTree.onTreeUpdate?.Invoke();
        }

        public void RunBehaviorTree(BehaviorTree behaviorTree)
        {
            if(behaviorTree != null)
            {
                if(behaviorTree != this.behaviorTree)
                {
                    // When there is a new behavior tree cancel all the updates to the previous tree
                    // and clone the new tree
                    CancelInvoke("ExecuteBehaviorTree");
                    this.behaviorTree = behaviorTree.Clone();
                }
                // Run behavior tree updates
                this.behaviorTree.treeState = EBehaviorTreeState.Running;
                InvokeRepeating("ExecuteBehaviorTree", 0f, UpdateInterval);
            }
        }
        
        private void ExecuteBehaviorTree()
        {
            // Begin the execution of the behavior tree
            behaviorTree.rootNode.Execute();
        }
    }

}
