using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class Test : MonoBehaviour
{
    public bool test = false;
    public Color color;
    private BehaviorTreeComponent behaviorTreeComponent;

    void Awake()
    {
       behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        behaviorTreeComponent.blackboard.SetBlackbordValue<bool>("CanSee", test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
