using BT.Runtime;
using UnityEngine;

public sealed class DisplaySprite : BT_ActionNode
{
    /// <summary>
    /// The sprite to display.
    /// </summary>
    public Sprite sprite;
    
    /// <summary>
    /// The sprite renderer used to render the target sprite.
    /// </summary>
    private SpriteRenderer spriteRenderer;
    
    // Called when the behavior tree wants to execute this action.
    // Modify the 'state' as you need, return SUCCESS when you want this node
    // to succeed, RUNNING when you want to notify the tree that this node is still running
    // and has not finished yet and FAILED when you want this node to fail
    protected override ENodeState Execute()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            state = ENodeState.Success;
        }
        else
        {
            state = ENodeState.Failed;
        }
        
        return state;
    }
    
    // Called to initialize this action properties before first update.
    protected override void OnInit()
    {
        GameObject obj = blackboard.GetBlackboardValueByKey<GameObject>("Owner");
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
    }
    
    // Called when the behavior tree starts executing this action
    protected override void OnStart()
    {
    }
    
    // Called when the behavior tree stops executing this action
    protected override void OnStop()
    {
    }
    
// Put here editor only logic
#if UNITY_EDITOR
    
    // for BT nodes the constructor is a editor only method, do not use it at runtime!
    private void OnEnable()
    {
        description = "Change a sprite renderer sprite and display it";
    }

#endif
}