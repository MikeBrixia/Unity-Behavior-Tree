
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    public sealed class BT_RootNode : BT_ParentNode
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

        protected override EBehaviorTreeState Execute()
        {
            return childNode.ExecuteNode();
        }

        ///<summary>
        /// Make a copy this root node asset.
        ///</summary>
        public override BT_Node Clone()
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

        public override List<T> GetChildNodes<T>()
        {
            return null;
        }

        public override List<BT_Node> GetChildNodes()
        {
            return new List<BT_Node> {childNode};
        }

        public override void AddChildNode<T>(T childNode)
        {
            throw new System.NotImplementedException();
        }

        public override void AddChildNode(BT_ParentNode child)
        {
            childNode = child;
        }

        public override void RemoveChildNode<T>(T child)
        {
            childNode = childNode.Equals(child) ? null : childNode;
        }

        public override Type[] GetNodeChildTypes()
        {
            return null;
        }

        public override void DestroyChildrenNodes()
        {
            throw new NotImplementedException();
        }

        public override void DestroyChild(BT_ChildNode child)
        {
            throw new NotImplementedException();
        }
    }
}

