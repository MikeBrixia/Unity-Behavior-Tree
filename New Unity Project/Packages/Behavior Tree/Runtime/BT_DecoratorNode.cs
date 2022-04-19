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

        /// <summary>
        /// Determines if a decorator is successfull or not
        /// </summary>
        public override EBehaviorTreeState Execute()
        {
           return EBehaviorTreeState.Success;
        }

    }
}

