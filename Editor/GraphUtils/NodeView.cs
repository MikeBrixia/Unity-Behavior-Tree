using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using BT.Runtime;

namespace BT.Editor
{
    
    ///<summary>
    /// Base class for all node views.
    ///</summary>
    public abstract class NodeView : Node
    {

        ///<summary>
        /// The node contained inside this node view
        ///</summary>
        public NodeBase node { get; protected set; }
        
        ///<summary>
        /// Set the position of this node view.
        ///</summary>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            // Constantly update node position in the graph with
            // Undo/redo support
            Undo.RecordObject(node, "Node position");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
    }


}

