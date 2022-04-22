using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class Wait : BT_ActionNode
    {

        public float time = 5f;

        private float startTime = 0f;

        // Called when the behavior tree wants to execute this action.
        // Modify the 'state' has you need, return SUCCESS when you want this node
        // to succed, RUNNING when you want to notify the tree that this node is still running
        // and has not finished yet and FAILED when you want this node to fail
        public override EBehaviorTreeState Execute()
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime >= time)
            {
                startTime = 0f;
                state = EBehaviorTreeState.Success;
            }
            else
            {
                state = EBehaviorTreeState.Running;
            }
            return state;
        }

        // Called when the behavior tree starts executing this action
        protected override void OnStart()
        {
            startTime = Time.time;
        }

        // Called when the behavior tree stops executing this action
        protected override void OnStop()
        {

        }

#if UNITY_EDITOR

        public Wait() : base()
        {
            description = "Wait for a given amount of time before continuing executing the branch";
        }
#endif
    }
}
