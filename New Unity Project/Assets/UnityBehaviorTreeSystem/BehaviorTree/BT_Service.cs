using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_Service : BT_Node
    {
        public float UpdateInterval = 0.5f;
        private float currentTimeCounter = 0f;

        public override ENodeState Execute()
        {
            return ENodeState.SUCCESS;
        }

        public override void OnStart()
        {
           tree.onTreeUpdate += ServiceUpdate;
        }

        public override void OnStop()
        {
           tree.onTreeUpdate -= ServiceUpdate;
        }
        
        // Handle service update frequency
        private void ServiceUpdate()
        {
            if(currentTimeCounter >= UpdateInterval)
            {
                currentTimeCounter = 0f;
                Execute();
            }
            else
            {
                currentTimeCounter += Time.deltaTime;
            }
        }
    }
}

