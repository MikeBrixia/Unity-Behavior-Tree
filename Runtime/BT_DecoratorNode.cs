using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    ///Decorators are nodes which can be attached to composite and action nodes 
    ///and are used as conditional nodes, to determine if a certain branch or task should be executed or not.
    ///</summary>
    public abstract class BT_Decorator : BT_ChildNode
    {
        /// <summary>
        /// Called when the behavior tree wants to execute this decorator. 
        /// Put here all the code you want this action to execute.
        ///</summary>
        ///<returns> SUCCESS if this decorator has been executed successfully, RUNNING if is still executing
        /// and FAILED if the action has failed to execute it's tasks.</returns>
        public override EBehaviorTreeState Execute()
        {
            return state;
        }
        
        ///<summary>
        /// Make a copy of this decorator asset
        ///</summary>
        ///<returns> A copy of this decorator asset.</returns>
        public override NodeBase Clone()
        {
            return base.Clone();
        }

        public override T GetParentNode<T>()
        {
            return null;
        }
    }
}

