using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT;

public class Distancetestscript : MonoBehaviour
{

    public GameObject player;
    
    private BehaviorTreeComponent behaviorTreeComponent;

    public bool Delay;
    
    void Awake()
    {
        behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        behaviorTreeComponent.blackboard.SetBlackbordValue<bool>("CanSee", true);
        if(Delay)
        {
            StartCoroutine("delay", 3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(3f);
        behaviorTreeComponent.blackboard.SetBlackbordValue<bool>("CanSee", false);
    }
}
