using System.Collections.Generic;

namespace BT.Runtime
{
    /// <summary>
    /// Interface implemented by Behavior Tree
    /// nodes which can have child nodes attached
    /// to them.
    /// </summary>
    public interface IParentNode
    {
        /// <summary>
        /// Get all the child nodes of this parent node.
        /// </summary>
        /// <typeparam name="T"> The type of the child node you want to get.</typeparam>
        /// <returns> A list of all child nodes of type T</returns>
        public IList<T> GetChildNodes<T>() where T : BT_Node, IChildNode;
        
        /// <summary>
        /// Add a child node to this parent node.
        /// </summary>
        /// <param name="childNode"> Child node you want to add</param>
        /// <typeparam name="T">The type of the child node you want to add</typeparam>
        public void AddChildNode<T>(T childNode) where T : BT_Node, IChildNode;
    }
}