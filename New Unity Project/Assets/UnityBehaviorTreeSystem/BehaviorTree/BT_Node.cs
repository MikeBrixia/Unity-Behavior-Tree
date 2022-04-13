using UnityEngine;
using UnityEditor;

namespace BT
{
    public abstract class BT_Node : NodeBase
    {
       
       [HideInInspector] 
       public Blackboard blackboard;
       
       internal BehaviorTree tree;

       ///<summary>
       /// Called when the node starts executing it's instructions
       ///</summary>
       public abstract void OnStart();
       
       ///<summary>
       /// Called when the node stop executing it's instructions
       ///</summary>
       public abstract void OnStop();

       ///<summary>
       /// Called when the behavior tree wants to execute this node
       ///</summary>
       public abstract ENodeState Execute();
       
       internal virtual void SetBehaviorTree(BehaviorTree behaviorTree)
       {
           this.tree = behaviorTree;
           this.blackboard = behaviorTree.blackboard;
       }
    }
}
    

