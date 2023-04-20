using BT.Runtime;
using UnityEngine;

namespace BT.Editor
{
    public struct NodeSelectionInfo
    {
        /// <summary>
        /// The selected node.
        /// </summary>
        public BT_Node node;
        
        /// <summary>
        /// Selection rectangle which has been used to select
        /// the graph element.
        /// </summary>
        public Rect selectionRectangle;

        public NodeSelectionInfo(BT_Node node, Rect selectionRectangle)
        {
            this.node = node;
            this.selectionRectangle = selectionRectangle;
        }
    }
}