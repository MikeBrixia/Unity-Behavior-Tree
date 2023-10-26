using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT.Runtime;
using UnityEditor;

namespace BT.Test
{
    public class Test : MonoBehaviour
    {
        public bool test;
        private BehaviorTreeComponent behaviorTreeComponent;
    
        void Awake()
        {
            behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        private void Update()
        {
            behaviorTreeComponent.blackboard.SetBlackboardValue("CanSee", test);
        }
    }
}
