using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public sealed class BT_RootNode : BT_Node
{

    [HideInInspector] public BT_Node childNode;

    public override ENodeState Execute()
    {
        return childNode.Execute();
    }

    public override void OnStart()
    {
        
    }

    public override void OnStop()
    {
        
    }

    public override NodeBase Clone()
    {
        BT_RootNode node = Instantiate(this);
        node.childNode = node.childNode.Clone() as BT_Node;
        return node;
    }

    internal override void SetBehaviorTree(BehaviorTree behaviorTree)
    {
        base.SetBehaviorTree(behaviorTree);
        childNode.SetBehaviorTree(behaviorTree);
    }
}
