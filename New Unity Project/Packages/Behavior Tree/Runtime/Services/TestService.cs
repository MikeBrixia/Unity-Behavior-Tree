using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class TestService : BT_Service
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    // Called each service update
    protected override void OnUpdate()
    {
        Debug.Log("Service Update");
    }

    // Called when this service becomes active and starts updating
    internal override void OnStart_internal()
    {
        
    }

    // Called when this service gets deactivated and stops updating
    internal override void OnStop_internal()
    {
        
    }
}