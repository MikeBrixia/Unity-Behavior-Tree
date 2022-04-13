using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{
    public abstract class BT_Decorator : BT_Node
    {
        
        public BT_Decorator()
        {
            
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }
        
        /// <summary>
        /// Determines if a decorator is successfull or not
        /// </summary>
        public override ENodeState Execute()
        {
           return ENodeState.SUCCESS;
        }

    }
}

