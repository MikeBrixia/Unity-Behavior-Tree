using System;
using System.Collections.Generic;
using BT.Runtime;
using Editor.BehaviorTree.BT_Elements;

namespace BT.Editor
{
    public static class BehaviorTreeManager
    {
        /// <summary>
        /// This map contains a list of nodes view mapped to their corresponding
        /// behavior tree node. For example BT_ActionNodeView will be mapped together
        /// with BT_ActionNode.
        /// </summary>
        public static readonly Dictionary<Type, Type> nodeViewMap = new Dictionary<Type, Type>()
        {
            [typeof(BT_ActionNode)] = typeof(BT_ActionView),
            [typeof(BT_Decorator)] = typeof(BT_DecoratorView),
            [typeof(BT_ServiceView)] = typeof(BT_ServiceView)
        };
        
        
    }
}