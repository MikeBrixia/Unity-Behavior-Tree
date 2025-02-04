# Composite

When creating a custom service node you will find the following inside the created .cs file:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT.Runtime;

public sealed class NewService : BT_Service
{
    // Called when this action gets initialized.
    protected override void OnInit()
    {
    }
    
    // Called when this service becomes active and starts updating
    protected override void OnStart()
    {
    }
    
    // Called when this service gets deactivated and stops updating
    protected override void OnStop()
    {
    }

    // Called each UpdateInterval tick
    protected override void OnUpdate()
    {
    }

// Put here editor only logic
#if UNITY_EDITOR
    
    public NewService() : base()
    {
        // Initialize editor-only properties here...
        description = "Node description";
    }

#endif
}
```

> [!NOTE]
> By default, service will be created as <b>sealed classes</b>; this is done to prevent developers from creating other nodes which derives from the current one. As a general rule, composites should only derive from: <b> BT_CompositeNode</b> type.

## Initialization

The <b>OnInit()</b> method will be called before the first behavior tree execution update and it is used to initialize properties.
It is called before the start function and only when the behavior tree gets clone/created at runtime.
the [RunBehaviorTree()](https://unity-behavior-tree-docs.netlify.app/api/bt.runtime.behaviortreecomponent#BT_Runtime_BehaviorTreeComponent_RunBehaviorTree_BT_Runtime_BehaviorTree_) method,
for example, will create a copy of the supplied behavior tree asset and run it, therefore triggering the OnInit() method on each of the cloned tree nodes.

```csharp
// Called when the behavior tree starts executing this action
protected override void OnInit()
{
    // Initialize properties here...
}
```

> [!TIP]
> You should use the OnInit() method only to initialize only one-time properties.
> For properties which could change over tree updates, please look at the OnStart() method.
> 
## Start

This is the method which gets called when the node begins executing, meaning that it will be called before the <b>Execute</b> function.

```csharp

// Called when the behavior tree starts executing this composite
protected override void OnStart()
{
    // Initialize properties here...
}
```

> [!TIP]
> Usually on start is used to initialize properties and retrieve key info from the blackboard.

## Stop

This method gets called when the composite fails or gets executed successfully.

```csharp

// Called when the behavior tree stops executing this action
protected override void OnStop()
{
}

```
## Execute

Execute is the most important function for composite nodes and it's used to perform the branch handling logic. Return type for this function it's also important and there can be three different types:

- SUCCESS when the service updates succesfully, you should always return this state.
- FAILED should not be returned.
- RUNNING should not be returned.
- WAITING is only used internally, i reccomend to never return this state because it could lead
          to unexpected behavior.

The [state member variable](https://unity-behavior-tree-docs.netlify.app/api/bt.runtime.bt_node#BT_Runtime_BT_Node_state) is used to handle and keep
track of nodes state through tree updates, so <b>when you want to modify the state of the node, you must also modify this variable</b>.

```csharp

// Called when the behavior tree wants to execute this service.
public override EBehaviorTreeState OnUpdate()
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

Here, for example, we're telling Unity to display "Node Description" as the default description for all nodes of type NewComposite when they get
created inside the editor graph.