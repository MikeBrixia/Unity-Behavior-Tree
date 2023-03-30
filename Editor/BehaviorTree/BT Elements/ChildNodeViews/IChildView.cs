

namespace BT.Editor
{
    public interface IChildView
    {
        /// <summary>
        /// Get the parent view of this child.
        /// </summary>
        /// <typeparam name="T"> The type of the parent view. </typeparam>
        /// <returns> The parent view. </returns>
        T GetParentView<T>() where T : BT_NodeView, IChildView;
    
    }
}
