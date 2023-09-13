using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// This composite node it's going to execute all it's children from left to right
    /// and stop when one fails, if all children nodes succeds this composite succeds, if even one
    /// Children fails this composite it's going to fail.
    ///</summary>
    public sealed class SequenceNode : BT_CompositeNode
    {
        public SequenceNode() : base()
        {
            nodeTypeName = "Sequence";
            description = "Execute all it's children in order and stops when one of them fails";
        }

        protected override ENodeState Execute()
        {
            BT_Node child = children[executionIndex];
            switch (child.ExecuteNode())
            {
                case ENodeState.Success:
                    executionIndex++;
                    break;
                case ENodeState.Running:
                    return ENodeState.Running;
                case ENodeState.Failed:
                     return ENodeState.Failed;
            }
            return executionIndex == children.Count? ENodeState.Success : ENodeState.Running;
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }
    }
}

