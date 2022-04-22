using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_CompositeNode : BT_Node
    {

        ///<summary>
        /// The children nodes this composite should try to execute
        ///</summary>
        [HideInInspector] public List<BT_Node> childrens = new List<BT_Node>();

        ///<summary>
        /// all the decorators attached to this node
        ///</summary>
        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();

        ///<summary>
        /// all the services attached to this node
        ///</summary>
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();

        protected int executedChildrenIndex = 0;

        internal override void OnStart_internal()
        {
            // Each time this node begins executing reset current executed children index
            executedChildrenIndex = 0;
            base.OnStart_internal();
        }

        internal override void OnStop_internal()
        {
            // When this node has done running disable all the service nodes updates
            services.ForEach(service => service.OnStop_internal());
            base.OnStop_internal();
        }

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

        public override NodeBase Clone()
        {
            BT_CompositeNode composite = Instantiate(this);
            composite.decorators = composite.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            composite.childrens = composite.childrens.ConvertAll(child => child.Clone() as BT_Node);
            composite.services = composite.services.ConvertAll(service => service.Clone() as BT_Service);
            return composite;
        }

        internal override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            decorators.ForEach(decorator => decorator.SetBlackboard(blackboard));
            services.ForEach(service => service.SetBlackboard(blackboard));
            childrens.ForEach(children => children.SetBlackboard(blackboard));
        }
    }
}

