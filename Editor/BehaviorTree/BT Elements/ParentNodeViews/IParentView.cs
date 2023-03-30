
using System.Collections.Generic;

namespace BT.Editor
{
    public interface IParentView
    {
        /// <summary>
        /// Get all the child views of the parent
        /// </summary>
        /// <typeparam name="T"> The type of the child views </typeparam>
        /// <returns> A list of all child views inside the parent. </returns>
        public IList<T> GetChildViews<T>() where T : BT_ChildNodeView;
        
        /// <summary>
        /// Add a child view to this parent view.
        /// </summary>
        /// <param name="childView"> The child you want to add. </param>
        /// <typeparam name="T"> The type of the child view. </typeparam>
        public void AddChildView<T>(T childView) where T : BT_ChildNodeView, IChildView;
        
        /// <summary>
        /// Create all child views using child nodes.
        /// </summary>
        public void CreateChildViews();
    }
}
