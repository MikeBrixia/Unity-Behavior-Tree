using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_Service : BT_Node
    {
        [Min(0)]
        [Tooltip("Interval at which this service is going to update")]
        public float updateInterval = 0.5f;
        
        [Tooltip("Call OnUpdate when the service execution starts")]
        public bool updateOnStart = false;

        private float startTime = 0f;

        public override EBehaviorTreeState Execute()
        {
            float elapsedTime = Time.time - startTime;
            if(elapsedTime >= updateInterval)
            {
                startTime = Time.time;
                OnUpdate();
                state = EBehaviorTreeState.Success;
            }
            else
            {
                state = EBehaviorTreeState.Running;
            }
            // service nodes doesn't need to care about Success or failure,
            // for this reason we are always gonna return success
            return state;
        }

        internal override void OnStart_internal()
        {
            base.OnStart_internal();
            startTime = Time.time;
            if(updateOnStart)
            {
               OnUpdate();
            }
        }

        internal override void OnStop_internal()
        {
            base.OnStop_internal();
            isStarted = false;
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

    #if UNITY_EDITOR

        public string frequencyDescription;
        
        public BT_Service() : base()
        {
            frequencyDescription = "Tick every: " + updateInterval + " seconds";
            description = "Service description";
        }

    #endif 
    }
}

