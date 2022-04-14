using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BT_ActionNode : BT_Node
    {

        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();
        
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();

        public override EBehaviorTreeState Execute()
        {
            return EBehaviorTreeState.Success;
        }

        public override void OnStart()
        {
            state = EBehaviorTreeState.Running;
            services.ForEach(service => service.OnStart());
        }

        public override void OnStop()
        {
            services.ForEach(service => service.OnStop());
        }

        public override EBehaviorTreeState ExecuteNode()
        {
            return DecoratorsSuccessfull()? base.ExecuteNode() : EBehaviorTreeState.Failed;
        }

        public bool DecoratorsSuccessfull()
        {
            bool decoratorConditions = true;
            foreach(BT_Decorator decorator in decorators)
            {
                EBehaviorTreeState DecoratorResult = decorator.Execute();
                if(DecoratorResult == EBehaviorTreeState.Failed)
                {
                    decoratorConditions = false;
                    break;
                }
            }
            return decoratorConditions;
        }

        public override NodeBase Clone()
        {
            BT_ActionNode action = Instantiate(this);
            action.decorators = action.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            return action;
        }

        internal override void SetBehaviorTree(BehaviorTree behaviorTree)
        {
            base.SetBehaviorTree(behaviorTree);
            decorators.ForEach(decorator => decorator.SetBehaviorTree(behaviorTree));
            services.ForEach(service => service.SetBehaviorTree(behaviorTree));
        }
    }
}

