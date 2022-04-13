using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class Wait : BT_ActionNode
{
    // Called when the behavior tree wants to execute this action
    public override ENodeState Execute()
    {
        return state;
    }
    
    // Called when the behavior tree starts executing this action
    public override void OnStart()
    {
        
    }
    
    // Called when the behavior tree stops executing this action
    public override void OnStop()
    {
        
    }
}