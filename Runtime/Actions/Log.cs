using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// Action node which logs a message to the unity console
    ///</summary>
    public class Log : BT_ActionNode
    {
        ///<summary>
        /// The log message
        ///</summary>
        public string debugMessage;
        
        ///<summary>
        /// Log a message to the Unity console
        ///</summary>
        protected override ENodeState Execute()
        {
            Debug.Log(debugMessage);
            return ENodeState.Success;
        }

        protected override void OnInit()
        {
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            description = "Print a message in the console";
        }
#endif
    }
}
