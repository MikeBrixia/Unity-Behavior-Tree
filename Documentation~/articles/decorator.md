# Decorator

When creationg a custom decorator node you will find the following inside the created .cs file:

```csharp

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT.Runtime;

public sealed class NewDecorator : BT_Decorator
{
    // Called when the behavior tree starts executing this decorator
    protected override void OnStart()
    {
        #NOTRIM#
    }
    
    // Called when the behavior tree stops executing this decorator
    protected override void OnStop()
    {
        #NOTRIM#
    }
    
    // Called when the behavior tree wants to execute this decorator.
    // Modify the 'state' as you need, return SUCCESS when you want this node
    // to succeed, RUNNING when you want to notify the tree that this node is still running
    // and has not finished yet and FAILED when you want this node to fail
    public override ENodeState Execute()
    {
        return state;
    }
    
// Put here editor only logic
#if UNITY_EDITOR
    
    public NewDecorator() : base()
    {
        // Initialize editor-only properties here...
        description = "Node description";
    }
#endif
}

```

> [!NOTE]
> By default, decorator will be created as <b>sealed classes</b>; this is done to prevent developers from creating other nodes which derives from the current one. As a general rule, composites should only derive from: <b>BT_Decorator</b> type.

## Start

This is the method which gets called when the node begins executing, meaning that it will be called before the <b>Execute</b> function.

```csharp

// Called when the behavior tree starts executing this decorator
protected override void OnStart()
{
    // Initialize properties here...
}

```

> [!TIP]
> Usually on start is used to initialize properties and retrieve key info from the blackboard.

## Stop

This method gets called when the decorator fails or gets executed successfully.

```csharp

// Called when the behavior tree stops executing this decorator
protected override void OnStop()
{
}

```

## Execute

Execute is the most important function for decorator nodes and it's used to perform the branch handling logic. Return type for this function it's also important and there can be three different types:

- SUCCESS when the decorator condition is met.
- FAILED when the decorator condition is not met.
- RUNNING when the decorator needs more than one update to evaluate.
- WAITING is only used internally, i reccomend to never return this state because it could lead
          to unexpected behavior.

The [state member variable](https://unity-behavior-tree-docs.netlify.app/api/bt.runtime.bt_node#BT_Runtime_BT_Node_state) is used to handle and keep
track of nodes state through tree updates, so <b>when you want to modify the state of the node, you must also modify this variable</b>.

```csharp

// Called when the behavior tree wants to execute this decorator.
public override EBehaviorTreeState Execute()
{
    // 1. Perform task...
    ...

    // 2. Depending on task result, update node state
    state = // Updated state.

    return state;
}

```

## Editor only

Code which should control how a certain node appears or which editor-only properties, 
should be placed inside the following pre-processor condition.

```csharp

// Put here editor only logic
#if UNITY_EDITOR
    
    // Editor-only code here...

#endif

```

Doing so, will ensure that nodes editor related properties will be stripped from release versions of the game.

If you want your nodes to have certains properties displayed by default inside the editor, you can initialize them
inside the constructor.

```csharp

// Initialize editor-only properties here...
public NewAction() : base()
{
    description = "Node description";
}

```

Here, for example, we're telling Unity to display "Node Description" as the default description for all nodes of type NewDecorator when they get
created inside the editor graph.