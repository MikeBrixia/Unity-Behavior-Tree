using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{

    public enum ENodeState { RUNNING, SUCCESS, FAILED, WAITING }
    
    public abstract class NodeBase : ScriptableObject
    {
       
       ///<summary>
       /// Unique identifier for the node
       ///</summary>
       [HideInInspector] public GUID guid;
       
       public string nodeName;
       
       public string description;
       
       ///<summary>
       /// The position of this node in the graph
       ///</summary>
       [HideInInspector] public Vector2 position;

       ///<summary>
       /// The current state of this specific node
       ///</summary>
       protected ENodeState state = ENodeState.RUNNING;

       public NodeBase()
       {
           nodeName = GetType().ToString();
       }

       public virtual NodeBase Clone()
       {
           return Instantiate(this);
       }
    }
}

