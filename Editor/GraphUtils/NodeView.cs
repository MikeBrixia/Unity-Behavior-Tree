using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BT.Editor
{

    public abstract class NodeView : Node
    {

        ///<summary>
        /// The node contained inside this node view
        ///</summary>
        public NodeBase node { get; protected set; }

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

