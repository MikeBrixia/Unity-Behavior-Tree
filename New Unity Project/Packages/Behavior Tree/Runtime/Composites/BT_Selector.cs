using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BT_Selector : BT_CompositeNode
    {
        public BT_Selector() : base()
        {
            nodeName = "Selector";
            description = "Execute all it's childrens in order and stops when one of them succeds";
        }

        public override EBehaviorTreeState Execute()
        {
            BT_Node child = childrens[executedChildrenIndex];
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

            return executedChildrenIndex == childrens.Count ? EBehaviorTreeState.Failed : EBehaviorTreeState.Running;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }
    }
}

