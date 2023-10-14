# Composite

When creationg a custom composite node you will find the following inside the created .cs file:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT.Runtime;

public sealed class NewComposite : BT_CompositeNode
{
    // Called when this composite starts executing.
    protected override void OnStart()
    {
    }
    
    // Called when this composite execution ends.
    protected override void OnStop()
    {
    }
    
    // Execute this composite logic.
    protected override ENodeState Execute()
    {
        // Put composite logic here...
        return ENodeState.Failed;
    }

// Put here editor only logic
#if UNITY_EDITOR
    
    // for BT nodes the constructor is a editor only method, do not use it at runtime!
    public #NewComposite#() : base()
    {
        description = "Node description";
    }

#endif

}
```

> ![!NOTE]
> By default, composites will be created as <b>sealed classes</b>; this is done to prevent developers from creating other nodes which derives from the current one. As a general rule, composites should only derive from: <b> BT_CompositeNode</b> type.

## Start

This is the method which gets called when the node begins executing, meaning that it will be called before the <b>Execute</b> function.

```csharp

// Called when the behavior tree starts executing this composite
protected override void OnStart()
{
    // Initialize properties here...
}
```

> ![!TIP]
> Usually on start is used to initialize properties and retrieve key info from the blackboard.

## Stop

This method gets called when the composite fails or gets executed successfully.

```csharp

// Called when the behavior tree stops executing this composite
protected override void OnStop()
{
}

```
## Execute

Execute is the most important function for composite nodes and it's used to perform the branch handling logic. Return type for this function it's also important and there can be three different types:

- SUCCESS when the composite has executed it's branch and respected it's own internal rules
- FAILED when the composite did not respect it's own internal rules and failed executing it's branch(for example, sequence fails when one of it's  children fails).
- RUNNING If the composite is still trying to execute it's branch.
- WAITING is only used internally, i reccomend to never return this state because it could lead
          to unexpected behavior.

The [state member variable](https://unity-behavior-tree-docs.netlify.app/api/bt.runtime.bt_node#BT_Runtime_BT_Node_state) is used to handle and keep
track of nodes state through tree updates, so <b>when you want to modify the state of the node, you must also modify this variable</b>.

```csharp

// Called when the behavior tree wants to execute this composite.
public override EBehaviorTreeState Execute()
{
    // 1. Perform task...
    ...

    // 2. Depending on task result, update node state
    state = // Updated state.

    return state;
}

```

### Execution index

Composite nodes can have children, lots of them; for this reason they need a way to execute them in the established order across several potential updates. This property determines which child should gets executed during a specific tree update and, on custom composites, it's the job of the developer to manage it. Here's an example on how our internal Sequence node uses Execution index to regulate it's branche execution:


```csharp

protected override ENodeState Execute()
{
    BT_Node child = children[executionIndex];
    switch (child.ExecuteNode())
    {
        case ENodeState.Success:
            executionIndex++;
            break;
        case ENodeState.Running:
            return ENodeState.Running;
        case ENodeState.Failed:
        return ENodeState.Failed;
    }
    
    return executionIndex == children.Count? ENodeState.Success : ENodeState.Running;
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

Here, for example, we're telling Unity to display "Node Description" as the default description for all nodes of type NewComposite when they get
created inside the editor graph.