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

        public Log() : base()
        {
            description = "Print a message in the console";
        }

        ///<summary>
        /// Log a message to the Unity console
        ///</summary>
        protected override EBehaviorTreeState Execute()
        {
            Debug.Log(debugMessage);
            return EBehaviorTreeState.Success;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }
    }
}
