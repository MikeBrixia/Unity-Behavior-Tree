using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BT_ActionNode : BT_Node
    {

        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();
        
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();

        public override ENodeState Execute()
        {
            return ENodeState.SUCCESS;
        }

        public override void OnStart()
        {
            state = ENodeState.RUNNING;
            services.ForEach(service => service.OnStart());
        }

        public override void OnStop()
        {
            services.ForEach(service => service.OnStop());
        }
        
        internal ENodeState ExecuteAction()
        {
            // Notify that we started executing this node
            OnStart();
            // In case all attached decorators are successfull execute this action
            state = DecoratorsSuccessfull()? Execute() : ENodeState.FAILED;
            // Whether it failed or not, notify that we finished executing this action
            OnStop();
            return state;
        }

        public bool DecoratorsSuccessfull()
        {
            foreach(BT_Decorator decorator in decorators)
            {
                ENodeState DecoratorResult = decorator.Execute();
                if(DecoratorResult == ENodeState.FAILED)
                {
                    return false;
                }
            }
            return true;
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

