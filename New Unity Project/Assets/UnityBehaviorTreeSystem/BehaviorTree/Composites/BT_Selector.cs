using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BT_Selector : BT_CompositeNode
    {
        public BT_Selector() : base()
        {
            description = "Execute all it's childrens in order and stops when one of them succeds";
        }

        // When a children node succeds, stop the execution of every children node
        protected override bool StopExecution(ENodeState CurrentState)
        {
            return CurrentState == ENodeState.SUCCESS;
        }

    }
}

