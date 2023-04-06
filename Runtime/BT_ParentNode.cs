using System;
using System.Collections.Generic;

namespace BT.Runtime
{
    /// <summary>
    /// Parent nodes are behavior trees who can
    /// have children nodes attached to them.
    /// </summary>
    public abstract class BT_ParentNode : BT_Node
    {
        /// <summary>
        /// Get all the child nodes of this parent node.
        /// </summary>
        /// <typeparam name="T"> The type of the child node you want to get.</typeparam>
        /// <returns> A list of all child nodes of type T</returns>
        public abstract List<T> GetChildNodes<T>() where T : BT_ChildNode;

        /// <summary>
        /// Add a child node to this parent node.
        /// </summary>
        /// <param name="childNode"> Child node you want to add</param>
        /// <typeparam name="T">The type of the child node you want to add</typeparam>
        public abstract void AddChildNode<T>(T childNode) where T : BT_ChildNode;
        
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