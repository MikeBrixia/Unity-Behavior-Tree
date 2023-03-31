using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// Behavior Tree action node.
    /// Actions are nodes with one input and no outputs, which are responsible
    /// of executing different tasks such as making the AI wait for a given amount
    /// of time or chasing the player.
    /// Actions can have decorators and services nodes attached to them and will execute 
    /// both before executing their own logic.
    ///</summary>
    public abstract class BT_ActionNode : BT_ParentNode
    {
        
        ///<summary>
        /// The decorators attached to this action
        ///</summary>
        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();
        
        ///<summary>
        /// The services attached to this action
        ///</summary>
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();
        
        ///<summary>
        /// Called when the behavior tree wants to execute this action. 
        /// Put here all the code you want this action to execute.
        ///</summary>
        ///<returns> SUCCESS if this action has been executed successfully, RUNNING if is still executing
        /// and FAILED if the action has failed to execute it's tasks.</returns>
        public override EBehaviorTreeState Execute()
        {
            return EBehaviorTreeState.Success;
        }
        
        ///<summary>
        /// Called when the Behavior Tree wants to start executing this action.
        /// This method will execute all decorators and if the result is successfull
        /// it will continue by executing first all services and then this action.
        ///</summary>
        ///<returns> The result of this action </returns>
        public override EBehaviorTreeState ExecuteNode()
        {
            // If all the decorators are successfull go ahead and execute 
            // all services and then the composite node
            if(ExecuteDecorators())
            {
                // Execute all service nodes attached to this composite
                services.ForEach(service => service.ExecuteNode());
                state = base.ExecuteNode();
            }
            else
            {
                state = EBehaviorTreeState.Failed;
            }
            return state;
        }
        
        ///<summary>
        /// Executes all decorators attached to this action
        ///</summary>
        ///<returns> True if all decorators are successfull, false otherwise</returns>
        private bool ExecuteDecorators()
        {
            bool decoratorsResult = true;
            // Execute all decorators attached to composite node
            foreach(BT_Decorator decorator in decorators)
            {
                state = decorator.ExecuteNode();
                if(state == EBehaviorTreeState.Failed
                   || state == EBehaviorTreeState.Running)
                {
                    decoratorsResult = false;
                    break;
                }
            }
            return decoratorsResult;
        }
        
        ///<summary>
        /// Internal version of OnStart() used to perform
        /// initialization.
        ///</summary>
        internal override void OnStart_internal()
        {
            state = EBehaviorTreeState.Running;
            base.OnStart_internal();
        }
        
        ///<summary>
        /// Internal version of OnStop() used to notify
        /// all services that this node has stopped executing.
        ///</summary>
        internal override void OnStop_internal()
        {
            services.ForEach(service => service.OnStop_internal());
            base.OnStop_internal();
        }
        
        ///<summary>
        /// Make a copy of this action asset
        ///</summary>
        ///<returns> A copy of this action asset</returns>
        public override NodeBase Clone()
        {
            BT_ActionNode action = Instantiate(this);
            action.decorators = action.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            action.services = action.services.ConvertAll(service => service.Clone() as BT_Service);
            return action;
        }
        
        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this action.
        ///</summary>
        ///<param name="blackboard">the blackboard used by the owner of this action</param>
        internal override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            decorators.ForEach(decorator => decorator.SetBlackboard(blackboard));
            services.ForEach(service => service.SetBlackboard(blackboard));
        }

        public override IList<T> GetChildNodes<T>()
        {
            IList<T> resultList = null;
            
            // Is T Decorator node type?
            if (typeof(T) == typeof(BT_Decorator))
                resultList = decorators as IList<T>;
            // Is T Service node type?
            else if (typeof(T) == typeof(BT_Service))
                resultList = services as IList<T>;
            
            return resultList;
        }

        public override void AddChildNode<T>(T childNode)
        {
            // Is T Decorator node type?
            if (typeof(T) == typeof(BT_Decorator))
                decorators.Add(childNode as BT_Decorator);
            // Is T Service node type?
            else if (typeof(T) == typeof(BT_Service))
                services.Add(childNode as BT_Service);
        }
    }
}

