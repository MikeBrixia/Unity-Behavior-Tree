using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    
    public class SequenceNode : BT_CompositeNode
    {
        public SequenceNode()
        {
            description = "Execute all it's childrens in order and stops when one of them fails";
        }

        // When a children of this node fails, stop the execution of this composite(make it fail)
        protected override bool StopExecution(ENodeState CurrentState)
        {
            return CurrentState == ENodeState.FAILED;
        }
    }
}

