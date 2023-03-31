namespace BT.Runtime
{
    /// <summary>
    /// Behavior Tree child nodes are nodes which can be
    /// attached to other parent nodes.
    /// </summary>
    public abstract class BT_ChildNode : BT_Node
    {
        /// <summary>
        /// Get the parent node of this child node.
        /// </summary>
        /// <typeparam name="T">The type of the parent node</typeparam>
        /// <returns>The parent node of type T</returns>
        public abstract T GetParentNode<T>() where T : BT_Node;
    }
}