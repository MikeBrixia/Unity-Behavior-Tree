using System;
using System.Collections;
using System.Collections.Generic;
using BT.Runtime;
using Editor.BehaviorTree.BT_Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    ///<summary>
    /// Handles the selection events in the behavior tree editor
    ///</summary>
    public static class BehaviorTreeManager
    {
        
        /// <summary>
        /// This map contains a list of nodes view mapped to their corresponding
        /// behavior tree node. For example BT_ActionNodeView will be mapped together
        /// with BT_ActionNode.
        /// </summary>
        public static readonly Dictionary<Type, Type> nodeViewMap = new Dictionary<Type, Type>()
        {
            [typeof(BT_RootNode)] = typeof(BT_ActionView),
            [typeof(BT_ActionNode)] = typeof(BT_ActionView),
            [typeof(BT_CompositeNode)] = typeof(BT_ActionView),
            [typeof(BT_Decorator)] = typeof(BT_DecoratorView),
            [typeof(BT_Service)] = typeof(BT_ServiceView)
        };
        
        ///<summary>
        /// The object on which the mouse is currently hovering
        ///</summary>
        public static ISelectable hoverObject { get; set; }
        
        ///<summary>
        /// The currently selected object
        ///</summary>
        public static ISelectable selectedObject { get; set; }
    }

}

