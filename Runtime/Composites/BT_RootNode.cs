using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public sealed class BT_RootNode : BT_Node
{

    [HideInInspector] public BT_Node childNode;

    public BT_RootNode() : base()
    {
        nodeName = "Root";
        description = "Entry point of behavior tree";
    }

    public override EBehaviorTreeState Execute()
    {
        return childNode.ExecuteNode();
    }

    public override NodeBase Clone()
    {
        BT_RootNode node = Instantiate(this);
        node.childNode = node.childNode.Clone() as BT_Node;
        return node;
    }
    
    internal override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        childNode.SetBlackboard(blackboard);
    }

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }
}
