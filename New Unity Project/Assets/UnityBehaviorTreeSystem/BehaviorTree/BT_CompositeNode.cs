using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BT_CompositeNode : BT_Node
    {
        [HideInInspector] public List<BT_Node> childrens = new List<BT_Node>();

        ///<summary>
        /// all the decorators attached to this node
        ///</summary>
        [HideInInspector] public List<BT_Decorator> decorators = new List<BT_Decorator>();
        
        ///<summary>
        /// all the services attached to this node
        ///</summary>
        [HideInInspector] public List<BT_Service> services = new List<BT_Service>();

        public override ENodeState Execute()
        {
            OnStart();
            // If even one decorator is not successfull make this node fail
            state = DecoratorsSuccessfull()? ExecuteChildrenNodes() : ENodeState.FAILED;
            return state;
        }
        
        // Execute all the children of this composite node.
        private ENodeState ExecuteChildrenNodes()
        {
            foreach (BT_Node Node in childrens)
            {
                BT_ActionNode actionNode = Node as BT_ActionNode;
                if (actionNode != null)
                {
                    state = actionNode.ExecuteAction();
                    if (StopExecution(state))
                    {
                        OnStop();
                        state = ENodeState.FAILED;
                        return state;
                    }
                }
                else
                {
                    // Execute children node
                    state = Node.Execute();
                    // If the executed node state is the right state to stop the execution of this node then
                    // Stop the execution and return it.
                    if (StopExecution(state))
                    {
                        OnStop();
                        state = ENodeState.FAILED;
                        return state;
                    }
                }
            }
            // When all the childrens get executed successfully return Success
            state = ENodeState.SUCCESS;
            return state;
        }

        public override void OnStart()
        {
            services.ForEach(service => service.OnStart());
            state = ENodeState.RUNNING;
        }

        public override void OnStop()
        {
            // When this decorator has done running disable all the service nodes updates
            services.ForEach(service => service.OnStop());
        }

        ///<summary>
        /// Condition which determines if the composite should stop execution or continue it
        ///</summary>
        protected abstract bool StopExecution(ENodeState CurrentState);

        private bool DecoratorsSuccessfull()
        {
            foreach (BT_Decorator Decorator in decorators)
            {
                ENodeState DecoratorResult = Decorator.Execute();
                if (DecoratorResult == ENodeState.FAILED)
                {
                    return false;
                }
            }
            return true;
        }

        public override NodeBase Clone()
        {
            BT_CompositeNode composite = Instantiate(this);
            composite.decorators = composite.decorators.ConvertAll(decorator => decorator.Clone() as BT_Decorator);
            composite.childrens = composite.childrens.ConvertAll(child => child.Clone() as BT_Node);
            return composite;
        }

        internal override void SetBehaviorTree(BehaviorTree behaviorTree)
        {
            base.SetBehaviorTree(behaviorTree);
            decorators.ForEach(decorator => decorator.SetBehaviorTree(behaviorTree));
            services.ForEach(service => service.SetBehaviorTree(behaviorTree));
            childrens.ForEach(children => children.SetBehaviorTree(behaviorTree));
        }
    }
}

