using System;
using System.Collections.Generic;

namespace BT.Runtime
{
    /// <summary>
    /// Parent nodes are nodes who can
    /// have children nodes attached to them
    /// and can be connected to other parent nodes
    /// by using input/output ports.
    /// This type of node can be placed and dragged inside
    /// the graph and hold a position in it.
    /// </summary>
    public abstract class BT_ParentNode : BT_Node
    {

#if true
        /// <summary>
        /// The tree top-down level of this node.
        /// </summary>
        public int level = -1;
#endif
        /// <summary>
        /// Get all the child nodes of this parent node.
        /// </summary>
        /// <typeparam name="T"> The type of the child node you want to get.</typeparam>
        /// <returns> A list of all child nodes of type T</returns>
        public abstract List<T> GetChildNodes<T>() where T : BT_ChildNode;
        
        /// <summary>
        /// Get a list of all the nodes connected to this parent node.
        /// </summary>
        /// <returns> All the nodes connected to this node. </returns>
        public abstract List<BT_ParentNode> GetConnectedNodes();
        
        /// <summary>
        /// Add a child node to this parent node.
        /// </summary>
        /// <param name="childNode"> Child node you want to add</param>
        /// <typeparam name="T">The type of the child node you want to add</typeparam>
        public abstract void AddChildNode<T>(T childNode) where T : BT_ChildNode;
        
        /// <summary>
        /// Connect this node to another node. When connected
        /// The node will become the parent of the "child" node.
        /// </summary>
        /// <param name="child"> The child node to connect to this parent node. </param>
        public abstract void ConnectNode(BT_ParentNode child);
        
        /// <summary>
        /// Remove a node connected to this parent node. Child node
        /// will be disconnected from this parent node and will no
        /// longer be a child of this node.
        /// </summary>
        /// <param name="child"> The node you want to disconnect.</param>
        public abstract void DisconnectNode(BT_ParentNode child);
        
        /// <summary>
        /// Get all the children types supported by this node.
        /// </summary>
        /// <returns> An array of all the supported children node types. </returns>
        public abstract Type[] GetNodeChildTypes();
        
        /// <summary>
        /// Destroy all child nodes attached to this parent.
        /// </summary>
        public abstract void DestroyChildrenNodes();
        
        /// <summary>
        /// Destroy the given child node of this parent.
        /// </summary>
        /// <param name="child"> The children to destroy</param>
        public abstract void DestroyChild(BT_ChildNode child);
    }
}