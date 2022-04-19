using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{

    public class SequenceNode : BT_CompositeNode
    {
        public SequenceNode()
        {
            nodeName = "Sequence";
            description = "Execute all it's childrens in order and stops when one of them fails";
        }

        public override EBehaviorTreeState Execute()
        {
            if (DecoratorsSuccessfull())
            {
                BT_Node child = childrens[executedChildrenIndex];
                switch (child.ExecuteNode())
                {
                    case EBehaviorTreeState.Success:
                        executedChildrenIndex++;
                        break;
                    case EBehaviorTreeState.Running:
                        return EBehaviorTreeState.Running;
                    case EBehaviorTreeState.Failed:
                        return EBehaviorTreeState.Failed;
                }
                
                return executedChildrenIndex == childrens.Count? EBehaviorTreeState.Success : EBehaviorTreeState.Running;
            }
            return EBehaviorTreeState.Failed;
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }
    }
}

