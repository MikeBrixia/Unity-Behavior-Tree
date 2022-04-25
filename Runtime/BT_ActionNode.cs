using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_ActionNode : BT_Node
    {

        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();
        
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();

        public override EBehaviorTreeState Execute()
        {
            return EBehaviorTreeState.Success;
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

        internal override void OnStart_internal()
        {
            state = EBehaviorTreeState.Running;
            base.OnStart_internal();
        }

        internal override void OnStop_internal()
        {
            services.ForEach(service => service.OnStop_internal());
            base.OnStop_internal();
        }

        public override NodeBase Clone()
        {
            BT_ActionNode action = Instantiate(this);
            action.decorators = action.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            action.services = action.services.ConvertAll(service => service.Clone() as BT_Service);
            return action;
        }

        internal override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            decorators.ForEach(decorator => decorator.SetBlackboard(blackboard));
            services.ForEach(service => service.SetBlackboard(blackboard));
        }
    }
}

