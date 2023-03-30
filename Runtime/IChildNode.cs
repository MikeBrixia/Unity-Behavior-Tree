namespace BT.Runtime
{
    /// <summary>
    /// Interface for nodes which can be
    /// attached to parent nodes.
    /// </summary>
    public interface IChildNode
    {
        /// <summary>
        /// Get the parent node of this child node.
        /// </summary>
        /// <typeparam name="T">The type of the parent node</typeparam>
        /// <returns>The parent node of type T</returns>
        public T GetParentNode<T>() where T : BT_Node;
    }
}