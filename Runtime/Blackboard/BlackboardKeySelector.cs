using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BT.Runtime
{
    [Serializable]
    public struct BlackboardKeySelector
    {
        /**
         * The key which identifies the blackboard
         * property/variable.
         */
        [SerializeField][HideInInspector]
        public string blackboardKey;
        
        /**
         * A type constrain which will force the blackboard key selector
         * to select only blackboard keys of the specified supported type.
         */
        [FormerlySerializedAs("typeConstrain")] public BlackboardSupportedTypes typeFilter;
    }
}