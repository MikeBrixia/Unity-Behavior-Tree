using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class Loop : BT_CompositeNode
    {
        public int loopTimes = 3;

        private int currentLoop = 0;
        
        public Loop() : base()
        {
            description = "Loop a given number of times before returning Success. If all the loops succeds this node succeds," +
                           " if even one loop fails this node it's going to fail";
        }

        public override EBehaviorTreeState Execute()
        {
            BT_Node child = childrens[executedChildrenIndex];
            switch (child.ExecuteNode())
            {
                case EBehaviorTreeState.Success:
                    executedChildrenIndex++;
                    state = EBehaviorTreeState.Running;
                    // If we executed all the loop childrens 
                    if (executedChildrenIndex == childrens.Count)
                    {
                        currentLoop++;
                        executedChildrenIndex = 0;
                        if (currentLoop == loopTimes)
                        {
                            state = EBehaviorTreeState.Success;
                            currentLoop = 0;
                        }
                    }
                    break;

                case EBehaviorTreeState.Running:
                    state = EBehaviorTreeState.Running;
                    break;

                case EBehaviorTreeState.Failed:
                    state = EBehaviorTreeState.Failed;
                    break;     
            }
            return state;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }
    }
}

