using System;
using System.Collections.Generic;
using BT.Runtime;
using UnityEngine;

namespace BT.Editor
{
    [Serializable]
    public struct CopyCache
    {
        /// <summary>
        /// The nodes we want to copy.
        /// </summary>
        public BT_ParentNodeView view;
        
        /// <summary>
        /// The direction from the selection center.
        /// </summary>
        public Vector2 direction;
        
        /// <summary>
        /// Distance between the node and the selection center.
        /// </summary>
        public float distance;
    }
}