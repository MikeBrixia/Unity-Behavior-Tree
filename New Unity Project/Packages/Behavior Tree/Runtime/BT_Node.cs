using UnityEngine;
using UnityEditor;

namespace BT
{
    public abstract class BT_Node : NodeBase
    {

        [HideInInspector]
        public Blackboard blackboard;
        
        [HideInInspector]
        public bool isStarted = false;

        internal BehaviorTree tree;

        ///<summary>
        /// Called when this has started executing it's instructions.
        /// that's the internal version and you should not override this
        /// for your game logic, use OnStart instead.
        ///</summary>
        internal virtual void OnStart_internal()
        {
            OnStart();
        }

        ///<summary>
        /// Called when this node has succeded or failed it's execution.
        /// that's the internal version and you should not override this
        /// for your game logic, use OnStop instead.
        ///</summary>
        internal virtual void OnStop_internal()
        {
            OnStop();
        }
        
        ///<summary>
        /// Called when this node has started it's execution
        ///</summary>
        protected abstract void OnStart();
        
        ///<summary>
        /// Called when this node has succeded or failed it's execution
        ///</summary>
        protected abstract void OnStop();

        ///<summary>
        /// Called when the behavior tree wants to execute this node
        ///</summary>
        public abstract EBehaviorTreeState Execute();
        
        ///<summary>
        /// Start the execution of this node
        ///</summary>
        public virtual EBehaviorTreeState ExecuteNode()
        {
            // Notify that the node has started executing
            if (!isStarted)
            {
                OnStart_internal();
                isStarted = true;
            }

            // Execute the node logic
            state = Execute();

            // If the node logic returned a success or failure notify that
            // the execution of this node has stopped
            if (state == EBehaviorTreeState.Success
               || state == EBehaviorTreeState.Failed)
            {
                OnStop_internal();
                isStarted = false;
            }

            return state;
        }

        internal virtual void SetBehaviorTree(BehaviorTree behaviorTree)
        {
            this.tree = behaviorTree;
            this.blackboard = behaviorTree.blackboard;
        }
    }
}


