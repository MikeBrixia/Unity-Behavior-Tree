using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class Log : BT_ActionNode
{
    
    public string debugMessage;

    public Log()
    {
        description = "Print a message in the console";
    }

    // Called when the behavior tree wants to execute this action
    public override EBehaviorTreeState Execute()
    {
        Debug.Log(debugMessage);
        return EBehaviorTreeState.Success;
    }

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }
}