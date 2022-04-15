using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT;

public class TestScript : MonoBehaviour
{

    public GameObject player;
    
    private BehaviorTreeComponent behaviorTreeComponent;

    public bool CanSee = false;
    public bool IsStunned = true;

    void Awake()
    {
        behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        behaviorTreeComponent.blackboard.SetBlackbordValue<bool>("CanSee", CanSee);
        behaviorTreeComponent.blackboard.SetBlackbordValue<bool>("IsStunned", IsStunned);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
