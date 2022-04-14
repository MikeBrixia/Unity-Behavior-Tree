using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BlackboardDecorator : BT_Decorator
    {

        private enum BlackboardDecoratorCondition { IsSetToFalse,  IsSetToTrue }
        
        public string blackboardKey;

        [SerializeField]
        private BlackboardDecoratorCondition condition = BlackboardDecoratorCondition.IsSetToTrue;

        public BlackboardDecorator() : base()
        {
            description = "Check the blackboard entry and returns success or fail based on the condition";
        }

        public override EBehaviorTreeState Execute()
        {
            bool conditionResult = blackboard.GetBlackboardValueByName<bool>(blackboardKey);
            if(condition == BlackboardDecoratorCondition.IsSetToTrue)
            {
                return conditionResult? EBehaviorTreeState.Success : EBehaviorTreeState.Failed; 
            }
            else
            {
                return !conditionResult? EBehaviorTreeState.Success : EBehaviorTreeState.Failed;
            }
        }
    }
}

