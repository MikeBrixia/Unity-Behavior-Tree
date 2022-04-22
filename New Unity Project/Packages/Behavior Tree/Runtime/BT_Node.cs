using UnityEngine;
using UnityEditor;

namespace BT
{
    public abstract class BT_Node : NodeBase
    {

        public Blackboard blackboard { get; set; }

        [HideInInspector]
        public bool isStarted = false;

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
            // If not already started, starts the execution
            StartExecution();
            // Execute the node logic
            state = Execute();
            // Once we've finished executing our instructions, determine 
            // if it's the case of stopping the execution
            StopExecution();

            return state;
        }

        internal void StartExecution()
        {
            // Notify that the node has started executing
            if (!isStarted)
            {
                OnStart_internal();
                isStarted = true;
            }
        }
        
        ///<summary>
        /// Try to stop the execution of this node, returns true
        /// if the execution was stopped, false otherwise.
        ///</summary>
        internal bool StopExecution()
        {
            // If the node logic returned a success or failure notify that
            // the execution of this node has stopped
            if (state == EBehaviorTreeState.Success
                || state == EBehaviorTreeState.Failed)
            {
                OnStop_internal();
                isStarted = false;
            }
            return !isStarted;
        }

        internal virtual void SetBlackboard(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
    }
}


