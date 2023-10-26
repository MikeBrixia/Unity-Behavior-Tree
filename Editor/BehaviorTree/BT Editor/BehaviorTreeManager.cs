
using UnityEditor.Experimental.GraphView;

namespace BT.Editor
{
    ///<summary>
    /// Handles the selection events in the behavior tree editor
    ///</summary>
    public static class BehaviorTreeManager
    {
        ///<summary>
        /// The object on which the mouse is currently hovering
        ///</summary>
        public static ISelectable hoverObject { get; set; }
        
        ///<summary>
        /// The currently selected object
        ///</summary>
        public static ISelectable selectedObject { get; set; }
    }

}

