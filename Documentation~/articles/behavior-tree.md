# Behavior Tree

## What is a Behavior Tree?

The Behavior Tree it's a tree data structure which updates at a defined frequency. The tree execute from top-left to bottom-right and has 4 types of nodes: <b>Action, Decorator, Composite and Service</b>.
On each update, depending on the frequency, the tree will advance it's execution from where it had left last update; This means that one tree traversal could execute nodes on different updates.

> [!NOTE]
> The fact that behavior tree traversal could take different updates, implies
  that <b>traversing all the Behavior Tree could require more than one update</b>.

## Default nodes types

### Parent nodes

Parents are nodes which can be placed, dragged inside the editor graph and, most importantly, which can have children nodes.

#### Action

Actions, also called leaf nodes, are nodes with one input and no outputs, which are responsible of executing different tasks such as making the AI wait for a given amount of time or chasing the player. Actions can have decorators and services attached to them which will <b>always</b> execute before their own logic. The action execution rules are the following:

1.  First execute all attached decorators, if they are all successful, then go ahead with step 2 and 3.
2.  Execute all attached services.
3.  Finally, execute the action node logic.

> [!TIP]
> Action nodes are usually used to make the AI character move or perform some kind of attack/task depending on the specific game and scenario.

#### Composite

Composites nodes are the <b>roots</b> of branches in the tree and define how a specific branch should execute and what rules should it follow. This nodes <b>have 1 input and multiple outputs(Childrens)</b>.
this nodes are usually used to create branches in the tree and execute a greater number of instructions.

> [!TIP]
> Most used composite nodes are, usually, selector and sequence nodes.

### Children nodes

Children are nodes which can only be attached to other parent nodes, meaning that they cannot have any child or be placed and dragged inside the editor.

#### Decorator

Decorators are also known as conditional nodes, and are used to control the flow of the tree, determining which branch or action should be executed or skipped. When attached to action nodes the decorator will decide whether or not that given action and it's attached services should execute, otherwise if placed on a composite it will decide whether or not that given branch and it's attached services should execute or not.

> [!TIP]
> You can use decorators, for example, to evaluate complex boolean expressions or check given blackboard values and return true or false depending on the condition(like in BlackboardDecorator built-in node).

#### Service

Services are parallel nodes which can be attached to composites and actions and will be executed at their defined frequency as long as their branch is being executed.
You can use service nodes to keep track of a value and update it at a defined frequency or when you want to ensure parallel node execution.

> [!NOTE]
> The behavior tree executes service as parallel nodes, this means that they will be executed on the same update as their parent.

> [!CAUTION]
> Service nodes are NOT multithread nodes by default!

> [!TIP]
> 1. A common use would be to to do some processing in a service node to update a blackboard key or other kind of value which will be needed by action nodes down in the execution.
> 2. If you need it, you can also define a custom tree-independent frequency update for each service node. The only constrain you have is
>    that custom frequency cannot be lesser than tree frequency.

