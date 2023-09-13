using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    ///Services are parallel nodes which can be attached to composites and actions and will be executed at their defined frequency 
    ///as long as their branch is being executed. 
    ///at the moment Services are NOT multithreaded by default!
    ///</summary>
    public abstract class BT_Service : BT_ChildNode
    {
        [Min(0)]
        [Tooltip("Interval at which this service is going to update")]
        ///<summary>
        /// The update frequency of this service. For example
        /// a service with and update frequency of 1 will execute 
        /// every second.
        ///</summary>
        public float updateInterval = 0.5f;
        
        ///<summary>
        /// If true this service will update when started, if false it will
        /// wait for the update interval to perform the first update.
        ///</summary>
        [Tooltip("Call OnUpdate when the service execution starts")]
        public bool updateOnStart = false;

        private float startTime = 0f;
        
        /// <summary>
        /// Called when the behavior tree wants to execute this service. 
        /// Put here all the code you want this service to execute.
        ///</summary>
        ///<returns> The return value of service nodes doesn't matter.</returns>
        protected override ENodeState Execute()
        {
            float elapsedTime = Time.time - startTime;
            if(elapsedTime >= updateInterval)
            {
                startTime = Time.time;
                OnUpdate();
                state = ENodeState.Success;
            }
            else
            {
                state = ENodeState.Running;
            }
            // service nodes doesn't need to care about Success or failure,
            // for this reason we are always gonna return success
            return state;
        }
        
        ///<summary>
        /// Internal version of OnStart(), used to perform
        /// initialization.
        ///</summary>
        internal override void OnStart_internal()
        {
            base.OnStart_internal();
            startTime = Time.time;
            if(updateOnStart)
            {
               OnUpdate();
            }
        }
        
        ///<summary>
        /// Used to notify that this service has stopped executing.
        /// executing.
        ///</summary>
        internal override void OnStop_internal()
        {
            base.OnStop_internal();
            isStarted = false;
        }
        
        ///<summary>
        /// Called when the Behavior Tree wants to execute this service, 
        /// This method will call OnStart_Internal if the service has not already started
        /// and then it will update this service.
        ///</summary>
        ///<returns> The result of this service(service results doesn't matter) </returns>
        public override ENodeState ExecuteNode()
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
        
        public override T GetParentNode<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}

