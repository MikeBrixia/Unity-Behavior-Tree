using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// Make the behavior tree wait for a given amount of time before
    /// continuing executing the next instructions.
    ///</summary>
    public class Wait : BT_ActionNode
    {
        ///<summary>
        /// The time the tree is going to wait
        ///</summary>
        public float time = 5f;
        
        /// <summary>
        /// The time from which we will start counting.
        /// </summary>
        private float startTime;

        // Called when the behavior tree wants to execute this action.
        // Modify the 'state' has you need, return SUCCESS when you want this node
        // to succeed, RUNNING when you want to notify the tree that this node is still running
        // and has not finished yet and FAILED when you want this node to fail
        protected override ENodeState Execute()
        {
            float elapsedTime = Time.time - startTime;
            state = elapsedTime >= time ? ENodeState.Success : ENodeState.Running;
            return state;
        }

        // Called when the behavior tree starts executing this action
        protected override void OnInit()
        {
        }

        protected override void OnStart()
        {
            startTime = Time.time;
        }

        // Called when the behavior tree stops executing this action
        protected override void OnStop()
        {
            startTime = 0f;
        }

#if UNITY_EDITOR

        public Wait() : base()
        {
            description = "Wait for a given amount of time before continuing executing the branch";
        }
#endif
    }
}
