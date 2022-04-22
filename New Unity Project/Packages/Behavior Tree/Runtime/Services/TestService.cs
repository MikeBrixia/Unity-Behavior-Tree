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

    #if UNITY_EDITOR
    
    public TestService() : base()
    {
        description = "Simple node for testing a service functionality";
    }

    #endif
}