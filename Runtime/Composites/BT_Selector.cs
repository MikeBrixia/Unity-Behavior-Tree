using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// This composite node will execute all children nodes from left to right
    /// and stop when one of them succeds.
    /// if all children fails, this node also fails.
    ///</summary>
    public sealed class BT_Selector : BT_CompositeNode
    {
        protected override ENodeState Execute()
        {
            BT_Node child = children[executionIndex];
            switch (child.ExecuteNode())
            {
                case ENodeState.Success:
                    return ENodeState.Success;

                case ENodeState.Failed:
                    executionIndex++;
                    break;

                case ENodeState.Running:
                    return ENodeState.Running;
            }

            return executionIndex == children.Count ? ENodeState.Failed : ENodeState.Running;
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
            nodeTypeName = "Selector";
            description = "Execute all it's children in order and stops when one of them succeeds";
        }
#endif
    }
}

