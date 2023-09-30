using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

namespace BT.Runtime
{
    public sealed class BT_RootNode : BT_Node
    {
        ///<summary>
        /// The root of the Behavior Tree, execution will start
        /// with this node
        ///</summary>
        [HideInInspector] public BT_Node childNode;

        public BT_RootNode() : base()
        {
            nodeTypeName = "Root";
            description = "Entry point of behavior tree";
        }

        public override EBehaviorTreeState Execute()
        {
            return childNode.ExecuteNode();
        }

        ///<summary>
        /// Make a copy this root node asset.
        ///</summary>
        public override NodeBase Clone()
        {
            BT_RootNode node = Instantiate(this);
            node.childNode = node.childNode.Clone() as BT_Node;
            return node;
        }

        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this root node.
        ///</summary>
        ///<param name="blackboard">the blackboard used by the owner of this root node</param>
        internal override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            childNode.SetBlackboard(blackboard);
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }
    }
}

