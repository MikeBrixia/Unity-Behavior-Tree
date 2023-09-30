using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BT.Runtime
{

    public abstract class NodeBase : ScriptableObject
    {
        
    #if (UNITY_EDITOR == true)
       ///<summary>
       /// Unique identifier for the node
       ///</summary>
       [HideInInspector] public GUID guid;
    #endif
        
        /// <summary>
        /// Custom node name which can be defined by the user.
        /// </summary>
        [FormerlySerializedAs("userDefinedName")] public string nodeName;
        
        /// <summary>
        /// The name type name of the node.
        /// </summary>
       [FormerlySerializedAs("nodeName")] [HideInInspector] public string nodeTypeName;
       
       /// <summary>
       /// Editable description of what this node does.
       /// </summary>
       public string description;
        
       ///<summary>
       /// The position of this node in the graph
       ///</summary>
       [HideInInspector] public Vector2 position;

       ///<summary>
       /// The current state of this specific node
       ///</summary>
       protected EBehaviorTreeState state = EBehaviorTreeState.Running;

       public NodeBase()
       {
           nodeTypeName = "(" + GetType() + ")";
       }

       public virtual NodeBase Clone()
       {
           return Instantiate(this);
       }
    }
}

