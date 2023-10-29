using System;
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
        [SerializeField] private BlackboardKeySelector key;
        
        ///<summary>
        /// The condition mode for this node.
        ///</summary>
        [SerializeField]
        private BlackboardDecoratorCondition condition = BlackboardDecoratorCondition.IsSetToTrue;
        
        protected override ENodeState Execute()
        {
            bool conditionResult = blackboard.GetBlackboardValueByKey<bool>(key.blackboardKey);
            if(condition == BlackboardDecoratorCondition.IsSetToTrue)
            {
                return conditionResult? ENodeState.Success : ENodeState.Failed; 
            }
            else
            {
                return !conditionResult? ENodeState.Success : ENodeState.Failed;
            }
        }

        protected override void OnInit()
        {
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }
        
#if UNITY_EDITOR

        private void OnEnable()
        {
            description = "Check the blackboard entry and returns success or fail based on the condition";
            // By default, a blackboard decorator should only show boolean types keys
            // since it is the only type it accepts.
            key.typeFilter = BlackboardSupportedTypes.Boolean;
        }
#endif
    }
}

