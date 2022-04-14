using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class Log : BT_ActionNode
{
    
    public string debugMessage;

    // Called when the behavior tree wants to execute this action
    public override EBehaviorTreeState Execute()
    {
        Debug.Log(debugMessage);
        return EBehaviorTreeState.Success;
    }
    
    // Called when the behavior tree starts executing this action
    public override void OnStart()
    {
        base.OnStart();
    }
    
    // Called when the behavior tree stops executing this action
    public override void OnStop()
    {
        base.OnStop();
    }
}