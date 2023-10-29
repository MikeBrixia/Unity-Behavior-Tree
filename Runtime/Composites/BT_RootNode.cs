
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
        [HideInInspector] public BT_ParentNode childNode;
        
        protected override ENodeState Execute()
        {
            return childNode.ExecuteNode();
        }

        ///<summary>
        /// Make a copy this root node asset.
        ///</summary>
        public override BT_Node Clone()
        {
            BT_RootNode node = (BT_RootNode) base.Clone();
            node.childNode = node.childNode.Clone() as BT_ParentNode;
            return node;
        }

        internal override void OnInit_internal()
        {
            base.OnInit_internal();
            
            // Initialize child node.
            childNode.OnInit_internal();
        }

        ///<summary>
        /// Set the blackboard component which is used by the tree who owns
        /// this root node.
        ///</summary>
        ///<param name="treeBlackboard">the blackboard used by the owner of this root node</param>
        public override void SetBlackboard(Blackboard treeBlackboard)
        {
            base.SetBlackboard(treeBlackboard);
            if (childNode != null)
            {
                childNode.SetBlackboard(treeBlackboard);
            }
        }

        protected override void OnInit()
        {
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            nodeTypeName = "Root";
            description = "Entry point of the behavior tree";
        }

        public override List<T> GetChildNodes<T>()
        {
            return null;
        }

        public override List<BT_ParentNode> GetConnectedNodes()
        {
            return new List<BT_ParentNode> {childNode};
        }

        public override void AddChildNode<T>(T childNode)
        {
            Debug.LogWarning("Root not cannot have children nodes attached to it.");
        }

        public override void ConnectNode(BT_ParentNode child)
        {
            childNode = child;
        }

        public override void DisconnectNode(BT_ParentNode child)
        { 
            childNode = childNode.Equals(child) ? null : childNode;
        }

        public override Type[] GetNodeChildTypes()
        {
            return null;
        }

        public override void DestroyChildrenNodes()
        {
            Debug.LogWarning("Root not cannot have children nodes attached to it.");
        }

        public override void DestroyChild(BT_ChildNode child)
        {
            Debug.LogWarning("Root not cannot have children nodes attached to it.");
        }
        
#endif
    }
}

