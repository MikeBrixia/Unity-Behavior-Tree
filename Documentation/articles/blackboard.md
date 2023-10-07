# Blackboard

## What is a Blackboard?

The Blackboard is the brain and interface between the tree and other game systems. It can store and access game data at runtime and it's used by the tree to make it's own decisions.
Blackboard properties can be retrieved by interacting with the blackboard, thus implying that all blackboard
properties can be used by behavior tree nodes.

> [!TIP]
> A common workflow is to gather needed blackboard properties when the node execution starts, and then use it for your own
  processing or to modify/update them.

The Blackboard is an asset which can be assigned, when needed, to one or more behavior tree assets.

> [!NOTE]
> When the game begins, each behavior tree will make a copy of their blackboard asset reference to avoid modifying it directly;
> In this way, each tree will have it's own independent blackboard state.

## Blackboard data

Blackboard stores data in the form of key-value-pairs, where each object value is associated to a string key. Properties can be accessed or setted
by interacting with the blackboard object, for more information check out the [Scripting API](https://unity-behavior-tree-docs.netlify.app/api/bt.runtime.blackboard).

> [!TIP]
> To avoid typo errors, Blackboard key selectors can be used to help developers select keys from a drop-down menu
  and filter them by desired type inside unity inspector.
  