using System;
using UnityEngine;
using UnityEditor;

namespace BT.Runtime
{
    
    ///<summary>
    /// Nodes possible states.
    ///</summary>
    public enum ENodeState { Running, Success, Failed, Waiting }
    
    ///<summary>
    /// Base class for all behavior tree nodes which
    /// contains the base logic for how a behavior tree
    /// node should behave.
    ///</summary>
    public abstract class BT_Node : ScriptableObject
    {
        
#if (UNITY_EDITOR == true)
        ///<summary>
        /// Unique identifier for the node
        ///</summary>
        [HideInInspector] public GUID guid;
        
        /// <summary>
        /// True if the node has been completed successfully during a tree update.
        /// </summary>
        public bool completed { get; protected set; }
#endif
        /// <summary>
        /// Custom node name which can be defined by the user.
        /// </summary>
        public string nodeName;
        
        /// <summary>
        /// The name type name of the node.
        /// </summary>
        [HideInInspector] public string nodeTypeName;
        
        /// <summary>
        /// Editable description of what this node does.
        /// </summary>
        [SerializeField] protected string description;
        
        ///<summary>
        /// The position of this node in the graph
        ///</summary>
        [HideInInspector] public Vector2 position;
        
        /// <summary>
        /// Reference to behavior tree blackboard.
        /// </summary>
        protected Blackboard blackboard;
        
        /// <summary>
        /// True if the node has started execution and not yet finished,
        /// false otherwise.
        /// </summary>
        [HideInInspector]
        public bool isStarted;
        
        ///<summary>
        /// Execution index which keeps track of which
        /// node this composite should try to execute during
        /// a tree update.
        ///</summary>
        public int executionIndex { get; protected set;  }
        
        ///<summary>
        /// The current state of this specific node
        ///</summary>
        public ENodeState state { get; protected set; }

        protected BT_Node()
        {
            nodeTypeName = "(" + GetType() + ")";
        }

        public virtual BT_Node Clone()
        {
            BT_Node clonedNode = Instantiate(this);
            clonedNode.guid = guid;
            return clonedNode;
        }

        ///<summary>
        /// Called when this node has started executing it's instructions.
        /// that's the internal version and you should not override this
        /// for your game logic, use OnStart instead.
        ///</summary>
        internal virtual void OnStart_internal()
        {
            OnStart();
        }

        ///<summary>
        /// Called when this node has succeeded or failed it's execution.
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
        /// Called when the behavior tree wants to execute this node. 
        /// Put here all the code you want this node to execute.
        ///</summary>
        ///<returns> SUCCESS if this node has been executed successfully, RUNNING if is still executing
        /// and FAILED if the node has failed to execute it's tasks.</returns>
        protected abstract ENodeState Execute();

        ///<summary>
        /// Called when the Behavior Tree wants to execute this node, 
        /// if not already started this node will start it's execution and
        /// call OnStart_Internal(), then it will execute this node instructions and 
        /// if the result was Success or Failed it will call OnStop_Internal().
        ///</summary>
        ///<returns> The result of this node </returns>
        public virtual ENodeState ExecuteNode()
        {
            // If not already started, starts the execution
            StartExecution();
            
            // Execute the node logic
            state = Execute();
            
            // Once we've finished executing our instructions, determine 
            // if it's the case of stopping the execution
            StopExecution();
            
            // If this node state is "Running", tell all it's parent nodes to wait for him to finish work.
            return state == ENodeState.Running? ENodeState.Waiting : state;
        }
        
        ///<summary>
        /// If this node has not started yet it will call OnStart_Internal().
        ///</summary>
        private void StartExecution()
        {
            // Notify that the node has started executing
            if (!isStarted)
            {
                OnStart_internal();
                isStarted = true;
                completed = false;
            }
        }
        
        ///<summary>
        /// Try to stop the execution of this node, returns true
        /// if the execution was stopped, false otherwise.
        ///</summary>
        private void StopExecution()
        {
            // If the node logic returned a success or failure notify that
            // the execution of this node has stopped
            if (state == ENodeState.Success
                || state == ENodeState.Failed)
            {
                OnStop_internal();
                isStarted = false;
                completed = true;
            }
        }
        
        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this node.
        ///</summary>
        ///<param name="treeBlackboard">the blackboard used by the owner of this node</param>
        public virtual void SetBlackboard(Blackboard treeBlackboard)
        {
            this.blackboard = treeBlackboard;
        }

        public Type GetNodeParentType()
        {
            return GetType().BaseType;
        }

        public Blackboard GetBlackboard()
        {
            return blackboard;
        }
    }
}


