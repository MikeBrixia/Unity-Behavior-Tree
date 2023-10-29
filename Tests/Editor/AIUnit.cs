using BT.Runtime;
using UnityEngine;

/// <summary>
/// A small script used just for testing out
/// behavior tree functionalities.
/// </summary>
public class AIUnit : MonoBehaviour
{
    /// <summary>
    /// Depending on if it is true or false, switch the sprite.
    /// </summary>
    public bool switchSprite;
    
    // BT object references
    private BehaviorTreeComponent behaviorTreeComponent;
    private Blackboard blackboard;
    
    private void Awake()
    {
        // Use the behavior tree component to access it's blackboard object reference.
        behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
        blackboard = behaviorTreeComponent.tree.blackboard;
        
        // Set this component owner(gameObject) as the owner of the
        // behavior tree. N.B. Owner it's just a property of the blackboard.
        blackboard.SetBlackboardValue("Owner", gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Check on each update if the switchSprite property has changed,
        // and keep updated the associated blackboard property.
        blackboard.SetBlackboardValue("ShowImage", switchSprite);
    }
}
