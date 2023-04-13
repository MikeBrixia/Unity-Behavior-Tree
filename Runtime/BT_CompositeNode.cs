using System;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// Composites nodes are the roots of branches in the tree and define how a specific branch
    /// should execute and what rules should it follow.
    /// This node have 1 input and multiple outputs(Childrens)
    ///</summary>
    public abstract class BT_CompositeNode : BT_ParentNode
    {

        ///<summary>
        /// The childrens this composite should try to execute.
        ///</summary>
        [HideInInspector] public List<BT_Node> childrens = new List<BT_Node>();

        ///<summary>
        /// Decorators attached to this composite
        ///</summary>
        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();

        ///<summary>
        /// Services attacehd to this composite
        ///</summary>
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();
        
        ///<summary>
        /// Execution index which keeps track of which
        /// node this composite should try to execute during
        /// a tree update.
        ///</summary>
        protected int executedChildrenIndex = 0;
        
        ///<summary>
        /// Internal version of OnStart(), used to perform
        /// initialization.
        ///</summary>
        internal override void OnStart_internal()
        {
            // Each time this node begins executing reset current executed children index
            executedChildrenIndex = 0;
            base.OnStart_internal();
        }
        
        ///<summary>
        /// Internal version of OnStop(), used to notify
        /// attached services that this composite has finished
        /// executing.
        ///</summary>
        internal override void OnStop_internal()
        {
            // When this node has done running disable all the service nodes updates
            services.ForEach(service => service.OnStop_internal());
            base.OnStop_internal();
        }
        
        ///<summary>
        /// Called when the Behavior Tree wants to execute this composite, 
        /// This method will execute all decorators and if the result is successfull
        /// it will continue by executing first all services and then this composite.
        ///</summary>
        ///<returns> The result of this composite </returns>
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
        /// Execute all decorators attached to this composite
        ///</summary>
        ///<returns> true if all decorators are successfull, false otherwise</returns>
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
        /// Make a copy of this composite asset
        ///</summary>
        ///<returns> A copy of this composite asset.</returns>
        public override BT_Node Clone()
        {
            BT_CompositeNode composite = Instantiate(this);
            composite.decorators = composite.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            composite.childrens = composite.childrens.ConvertAll(child => child.Clone());
            composite.services = composite.services.ConvertAll(service => service.Clone() as BT_Service);
            return composite;
        }
        
        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this composite.
        ///</summary>
        ///<param name="blackboard">the blackboard used by the owner of this composite</param>
        internal override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            decorators.ForEach(decorator => decorator.SetBlackboard(blackboard));
            services.ForEach(service => service.SetBlackboard(blackboard));
            childrens.ForEach(children => children.SetBlackboard(blackboard));
        }

        public override List<T> GetChildNodes<T>()
        {
            List<T> resultList = null;
            // Is T Decorator node type?
            if (typeof(T) == typeof(BT_Decorator))
                resultList = decorators as List<T>;
            // Is T Service node type?
            else if (typeof(T) == typeof(BT_Service))
                resultList = services as List<T>;
            return resultList;
        }

        public override List<BT_Node> GetConnectedNodes()
        {
            return childrens;
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
            childrens.Add(child);
        }

        public override void DisconnectNode(BT_ParentNode child)
        {
            if (child.GetType().IsSubclassOf(typeof(BT_ParentNode)))
            {
                childrens.Remove(child);
            }
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
    }
}

