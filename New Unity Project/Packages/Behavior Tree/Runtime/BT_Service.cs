using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_Service : BT_Node
    {
        [Min(0)]
        public float updateInterval = 0.5f;
        
        private float currentTimeCounter = 0f;
        
        public override EBehaviorTreeState Execute()
        {
            // Handle service update intervals
            if (currentTimeCounter >= updateInterval)
            {
                currentTimeCounter = 0f;
                OnUpdate();
            }
            else
            {
               float timeIncrease = tree.canTick? Time.deltaTime : tree.updateInterval;
               currentTimeCounter += timeIncrease;
            }
            // service nodes doesn't need to care about Success or failure,
            // for this reason we are always gonna return success
            return EBehaviorTreeState.Success;
        }

        public override EBehaviorTreeState ExecuteNode()
        {
            if (!isStarted)
            {
                OnStart_internal();
                isStarted = true;
            }
            state = Execute();
            return state;
        }

        ///<summary>
        /// Called each update at UpdateInterval time
        ///</summary>
        protected abstract void OnUpdate();
    }
}

