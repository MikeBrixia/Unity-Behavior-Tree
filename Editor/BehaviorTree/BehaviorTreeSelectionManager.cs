using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Editor
{

    ///<summary>
    /// Handles the selection events in the behavior tree editor
    ///</summary>
    public static class BehaviorTreeSelectionManager
    {

        ///<summary>
        /// The object on which the mouse is currently hovering
        ///</summary>
        public static object hoverObject { get; set; }

        ///<summary>
        /// The currently selected object
        ///</summary>
        public static object selectedObject { get; set; }
        
    }

}

