using System;
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
        [HideInInspector] public List<BT_Decorator> decorators = new();
        
        ///<summary>
        /// The services attached to this action
        ///</summary>
        [HideInInspector] public List<BT_Service> services = new();
        
        ///<summary>
        /// Called when the behavior tree wants to execute this action. 
        /// Put here all the code you want this action to execute.
        ///</summary>
        ///<returns> SUCCESS if this action has been executed successfully, RUNNING if is still executing
        /// and FAILED if the action has failed to execute it's tasks.</returns>
        protected override ENodeState Execute()
        {
            return ENodeState.Success;
        }
        
        ///<summary>
        /// Called when the Behavior Tree wants to start executing this action.
        /// This method will execute all decorators and if the result is successfull
        /// it will continue by executing first all services and then this action.
        ///</summary>
        ///<returns> The result of this action </returns>
        public override ENodeState ExecuteNode()
        {
            // If all the decorators are successful go ahead and execute 
            // all services and then the composite node
            if(ExecuteDecorators())
            {
                // Execute all service nodes attached to this composite
                services.ForEach(service => service.ExecuteNode());
                state = base.ExecuteNode();
            }
            else
            {
                state = ENodeState.Failed;
            }
            return state;
        }
        
        ///<summary>
        /// Executes all decorators attached to this action
        ///</summary>
        ///<returns> True if all decorators are successful, false otherwise</returns>
        private bool ExecuteDecorators()
        {
            bool decoratorsResult = true;
            // Execute all decorators attached to composite node
            foreach(BT_Decorator decorator in decorators)
            {
                state = decorator.ExecuteNode();
                if(state == ENodeState.Failed
                   || state == ENodeState.Running)
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
            state = ENodeState.Running;
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
        public override BT_Node Clone()
        {
            BT_ActionNode action = (BT_ActionNode) base.Clone();
            action.decorators = action.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            action.services = action.services.ConvertAll(service => service.Clone() as BT_Service);
            return action;
        }
        
        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this action.
        ///</summary>
        ///<param name="treeBlackboard">the blackboard used by the owner of this action</param>
        public override void SetBlackboard(Blackboard treeBlackboard)
        {
            base.SetBlackboard(treeBlackboard);
            decorators.ForEach(decorator => decorator?.SetBlackboard(treeBlackboard));
            services.ForEach(service => service?.SetBlackboard(treeBlackboard));
        }
        
#if UNITY_EDITOR
        
        public override List<T> GetChildNodes<T>()
        {
            List<T> resultList = new List<T>();
            // Is T Decorator node type?
            if (typeof(T) == typeof(BT_Decorator))
                resultList = decorators as List<T>;
            // Is T Service node type?
            else if (typeof(T) == typeof(BT_Service))
                resultList = services as List<T>;
            return resultList;
        }

        public override List<BT_ParentNode> GetConnectedNodes()
        {
            return new List<BT_ParentNode>();
        }

        public override void AddChildNode<T>(T childNode)
        {
            // The base type of the node.
            Type nodeType = childNode.GetType().BaseType;
            // Is T Decorator node type?
            if (nodeType == typeof(BT_Decorator))
                decorators.Add(childNode as BT_Decorator);
            // Is T Service node type?
            else if (nodeType == typeof(BT_Service))
                services.Add(childNode as BT_Service);
        }

        public override void ConnectNode(BT_ParentNode child)
        {
            Debug.LogWarning("Action nodes cannot have children");
        }

        public override void DisconnectNode(BT_ParentNode child)
        {
            Debug.LogWarning("Action nodes cannot have children");
        }

        public override Type[] GetNodeChildTypes()
        {
            return new Type[]
            {
                typeof(BT_Decorator),
                typeof(BT_Service)
            };
        }

        public override void DestroyChildrenNodes()
        {
            decorators.ForEach(decorator => UnityEditor.Undo.DestroyObjectImmediate(decorator));
            services.ForEach(service => UnityEditor.Undo.DestroyObjectImmediate(service));
        }
        
        public override void DestroyChild(BT_ChildNode child)
        {
            Type nodeParentType = child.GetNodeParentType();
            if (nodeParentType == typeof(BT_Decorator))
            {
                decorators.Remove((BT_Decorator) child);
            }
            else if (nodeParentType == typeof(BT_Service))
            {
                services.Remove((BT_Service) child);
            }
        }
#endif
    }
}

