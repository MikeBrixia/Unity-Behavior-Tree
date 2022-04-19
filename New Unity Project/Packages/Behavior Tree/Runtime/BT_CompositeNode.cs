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
            // Call on start on each service node attached
            services.ForEach(service => service.OnStart_internal());
            base.OnStart_internal();
        }
        
        internal override void OnStop_internal()
        {
            // When this node has done running disable all the service nodes updates
            foreach(BT_Service service in services)
            {
                service.isStarted = false;
                service.OnStop_internal();
            }
            base.OnStop_internal();
        }

        public override EBehaviorTreeState ExecuteNode()
        {
            if(DecoratorsSuccessfull())
            {
                services.ForEach(service => service.ExecuteNode());
                return base.ExecuteNode();
            }
            return EBehaviorTreeState.Failed;
        }

        protected bool DecoratorsSuccessfull()
        {
            foreach (BT_Decorator Decorator in decorators)
            {
                EBehaviorTreeState DecoratorResult = Decorator.Execute();
                if (DecoratorResult == EBehaviorTreeState.Failed)
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

