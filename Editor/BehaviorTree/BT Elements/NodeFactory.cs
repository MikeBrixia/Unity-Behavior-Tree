using System;
using BT.Editor;
using BT.Runtime;
using UnityEditor;

namespace BT
{
    /// <summary>
    /// Factory responsible of creating BT nodes and
    /// BT Nodes views.
    /// </summary>
    public static class NodeFactory
    {
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node which will be wrapped inside the new node view </param>
        public static void CreateNodeView<T>(BT_Node node, BehaviorTreeGraphView graph) where T : BT_NodeView, IParentView
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            
            // Create node view of type T.
            BT_NodeView nodeView = (BT_NodeView)Activator.CreateInstance(typeof(T),node, graph);
            
            // Create decorators views for composite and action nodes
            if(nodeView is IParentView parentView)
            {
                parentView.CreateChildViews();
            }
            
            // Setup selection callback on the node view to be the same
            nodeView.OnNodeSelected += graph.OnNodeSelected;
            graph.AddElement(nodeView);
        }
        
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node which will be wrapped inside the new node view </param>
        public static void CreateNodeView(BT_Node node, BehaviorTreeGraphView graph)
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            
            // Create a node view of the type associated with the node type.
            Type viewType = BehaviorTreeManager.nodeViewMap[node.GetType()];
            BT_NodeView nodeView = (BT_NodeView)Activator.CreateInstance(viewType,node, graph);
            
            // Create decorators views for composite and action nodes
            if(nodeView is IParentView parentView)
            {
                parentView.CreateChildViews();
            }
            
            // Setup selection callback on the node view to be the same
            nodeView.OnNodeSelected += graph.OnNodeSelected;
            graph.AddElement(nodeView);
        }
        
        public static void CreateChildNodeView(BT_Node parent, BT_Node childNode, BehaviorTreeGraphView graph) 
        {
            // When guid is invalid, generate a brand new one
            if (childNode.guid.Empty())
            {
                childNode.guid = GUID.Generate();
            }
            
            // Create a node view of the type associated with the node type.
            Type viewType = BehaviorTreeManager.nodeViewMap[childNode.GetType()];
            BT_NodeView nodeView = (BT_NodeView)Activator.CreateInstance(viewType, childNode, graph);
            
        }
    }
}

