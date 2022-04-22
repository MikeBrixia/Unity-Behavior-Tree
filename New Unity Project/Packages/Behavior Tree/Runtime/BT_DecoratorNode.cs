using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{
    public abstract class BT_Decorator : BT_Node
    {
        
        /// <summary>
        /// Execute this decorator
        /// </summary>
        public override EBehaviorTreeState Execute()
        {
            return state;
        }

        public override NodeBase Clone()
        {
            return base.Clone();
        }
    }
}

