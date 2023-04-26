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
        public BT_Selector() : base()
        {
            nodeTypeName = "Selector";
            description = "Execute all it's children in order and stops when one of them succeds";
        }

        protected override EBehaviorTreeState Execute()
        {
            BT_Node child = children[executedChildrenIndex];
            switch (child.ExecuteNode())
            {
                case EBehaviorTreeState.Success:
                    return EBehaviorTreeState.Success;

                case EBehaviorTreeState.Failed:
                    executedChildrenIndex++;
                    break;

                case EBehaviorTreeState.Running:
                    return EBehaviorTreeState.Running;
            }

            return executedChildrenIndex == children.Count ? EBehaviorTreeState.Failed : EBehaviorTreeState.Running;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }
    }
}

