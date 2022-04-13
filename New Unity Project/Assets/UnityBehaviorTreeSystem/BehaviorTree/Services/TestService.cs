using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class TestService : BT_Service
{

    // Called each UpdateInterval tick
    public override ENodeState Execute()
    {
        Debug.Log("Updating...");
        return state;
    }
    
    // Called when this service becomes active and starts updating
    public override void OnStart()
    {
        base.OnStart();
    }
    
    // Called when this service gets deactivated and stops updating
    public override void OnStop()
    {
        base.OnStop();
    }
}