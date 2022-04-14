using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class TestService : BT_Service
{

    // Called each service update
    protected override void OnUpdate()
    {
        Debug.Log("Service Update");
    }

    // Called when this service becomes active and starts updating
    public override void OnStart()
    {
        
    }

    // Called when this service gets deactivated and stops updating
    public override void OnStop()
    {
        
    }
}