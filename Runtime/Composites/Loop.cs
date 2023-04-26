using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// This composite node it's gonna loop a given number of times
    /// through it's children before returning success. If all loops are 
    /// successfull this node is going to succed, if even one loop fails
    /// this node it's going to fail.
    ///</summary>
    public sealed class Loop : BT_CompositeNode
    {
        ///<summary>
        /// The number of loops you want this composite
        /// to perform.
        ///</summary>
        [Tooltip("The number of loops you want this composite to perform.")]
        public int loopNumber = 3;
        
        ///<summary>
        /// The loop we're currently executing
        ///</summary>
        private int currentLoop = 0;
        
        public Loop() : base()
        {
            description = "Loop a given number of times before returning Success. If all the loops succeds this node succeds," +
                           " if even one loop fails this node it's going to fail";
        }

        protected override EBehaviorTreeState Execute()
        {
            BT_Node child = children[executedChildrenIndex];
            switch (child.ExecuteNode())
            {
                case EBehaviorTreeState.Success:
                    executedChildrenIndex++;
                    state = EBehaviorTreeState.Running;
                    // If we executed all the loop children 
                    if (executedChildrenIndex == children.Count)
                    {
                        currentLoop++;
                        executedChildrenIndex = 0;
                        if (currentLoop == loopNumber)
                        {
                            state = EBehaviorTreeState.Success;
                            currentLoop = 0;
                        }
                    }
                    break;

                case EBehaviorTreeState.Running:
                    state = EBehaviorTreeState.Running;
                    break;

                case EBehaviorTreeState.Failed:
                    state = EBehaviorTreeState.Failed;
                    break;     
            }
            return state;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }
    }
}

