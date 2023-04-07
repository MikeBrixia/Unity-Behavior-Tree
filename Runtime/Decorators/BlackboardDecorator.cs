using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// This decorator it's going to check a blackboard
    /// Boolean value and return Success if the bool value is true, 
    /// false otherwise.
    ///</summary>
    public sealed class BlackboardDecorator : BT_Decorator
    {
        ///<summary>
        /// Choose how to evaluate the boolean value, when set to false
        /// will return true if boolean value is false, when set to true will return
        /// true if the boolean value is true.
        ///</summary>
        private enum BlackboardDecoratorCondition { IsSetToFalse,  IsSetToTrue }
        
        ///<summary>
        /// The blackboard key those value is needed to evaluate the condition.
        ///</summary>
        public string blackboardKey;
        
        ///<summary>
        /// The condition mode for this node.
        ///</summary>
        [SerializeField]
        private BlackboardDecoratorCondition condition = BlackboardDecoratorCondition.IsSetToTrue;

        public BlackboardDecorator() : base()
        {
            description = "Check the blackboard entry and returns success or fail based on the condition";
        }

        protected override EBehaviorTreeState Execute()
        {
            bool conditionResult = blackboard.GetBlackboardValueByKey<bool>(blackboardKey);
            if(condition == BlackboardDecoratorCondition.IsSetToTrue)
            {
                return conditionResult? EBehaviorTreeState.Success : EBehaviorTreeState.Failed; 
            }
            else
            {
                return !conditionResult? EBehaviorTreeState.Success : EBehaviorTreeState.Failed;
            }
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }
    }
}

