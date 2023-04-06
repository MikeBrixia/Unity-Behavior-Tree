using System;
using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEngine;

namespace BT
{
    /// <summary>
    /// Factory responsible of creating and destroying BT nodes and
    /// BT Nodes views.
    /// </summary>
    public static class NodeFactory
    {
        public static BT_Node CreateNode(Type nodeType, BehaviorTree tree)
        {
            // Create node and generate GUID
            BT_Node node = ScriptableObject.CreateInstance(nodeType) as BT_Node;
            if (node != null)
            {
                node.nodeTypeName = nodeType.Name;
                node.guid = GUID.Generate();
            
                // If the new node is a root node, override
                // the old root node with the new one.
                if(nodeType == typeof(BT_RootNode))
                {
                    tree.rootNode = node as BT_RootNode;
                }
            
                // Register the node in the behavior tree.
                RegisterNode(node, tree);
            }
            return node;
        }
        
        public static T CreateNode<T>(BehaviorTree tree) where T : BT_Node
        {
            // The type of the node we want to create.
            Type nodeType = typeof(T);
                
            // Create node and generate GUID
            T node = ScriptableObject.CreateInstance<T>();
            node.nodeTypeName = nodeType.Name;
            node.guid = GUID.Generate();
            
            // If the new node is a root node, override
            // the old root node with the new one.
            if(nodeType == typeof(BT_RootNode))
            {
                tree.rootNode = node as BT_RootNode;
            }
            
            // Register the node in the behavior tree.
            RegisterNode(node, tree);
            return node;
        }

        public static BT_Node CreateChildNode(Type nodeType, BT_ParentNode parent, BehaviorTree tree)
        {
            // Create node and generate GUID
            BT_ChildNode node = ScriptableObject.CreateInstance(nodeType) as BT_ChildNode;
            if (node != null)
            {
                node.nodeTypeName = nodeType.Name;
                node.guid = GUID.Generate();
            
                // Register the node in the behavior tree.
                RegisterNode(node, tree);
                
                // Register child node inside parent node
                RegisterChildNode(node, parent);
                
            }
            return node;
        }
        
        /// <summary>
        /// Create a child node and attach it to it's parent.
        /// </summary>
        /// <param name="parent"> The parent node of the child. </param>
        /// <param name="tree"> The Behavior Tree on which the node will be created. </param>
        /// <typeparam name="TChildType"> The type of the child node. </typeparam>
        /// <typeparam name="TParentType"> The type of the parent node. </typeparam>
        /// <returns></returns>
        private static TChildType CreateChildNode<TChildType, TParentType>(TParentType parent, BehaviorTree tree) where TChildType : BT_ChildNode
                                                                                                                  where TParentType : BT_ParentNode
        {
            // Create node and generate GUID
            TChildType node = ScriptableObject.CreateInstance<TChildType>();
            node.nodeTypeName = typeof(TChildType).Name;
            node.guid = GUID.Generate();
            
            // Register the node in the behavior tree.
            RegisterNode(node, tree);
            
            // Register child node inside parent node
            RegisterChildNode(node, parent);

            return node;
        }
        
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node which will be wrapped inside the new node view </param>
        public static T CreateNodeView<T>(BT_Node node, BehaviorTreeGraphView graph) where T : BT_NodeView, IParentView
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            
            // Create node view of type T.
            T nodeView = (T)Activator.CreateInstance(typeof(T),node, graph);
            
            // Create decorators views for composite and action nodes
            if(nodeView is IParentView parentView)
            {
                parentView.CreateChildViews();
            }
            
            // Setup node selection callback.
            nodeView.onNodeSelected = graph.onNodeSelected;
            
            return nodeView;
        }
        
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node which will be wrapped inside the new node view </param>
        public static BT_NodeView CreateNodeView(BT_Node node, BehaviorTreeGraphView graph)
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            
            // TO CHANGE.
            Type nodeType = node.GetType();
            nodeType = nodeType == typeof(BT_RootNode) ? typeof(BT_RootNode) : nodeType.BaseType;
            
            // Create a node view of the type associated with the node type.
            Type viewType = BehaviorTreeManager.nodeViewMap[nodeType!];
            BT_NodeView nodeView = (BT_NodeView)Activator.CreateInstance(viewType,node, graph);
            
            // Create decorators views for composite and action nodes
            if(nodeView is IParentView parentView)
            {
                parentView.CreateChildViews();
            }
            
            // Setup node selection callback.
            nodeView.onNodeSelected = graph.onNodeSelected;
            
            return nodeView;
        }
        
        public static BT_ChildNodeView CreateChildNodeView(BT_ParentNodeView parent, BT_ChildNode childNode, BehaviorTreeGraphView graph) 
        {
            // When guid is invalid, generate a brand new one
            if (childNode.guid.Empty())
            {
                childNode.guid = GUID.Generate();
            }
            
            // Create child node view.
            Type viewType = BehaviorTreeManager.nodeViewMap[childNode.GetType().BaseType!];
            BT_ChildNodeView childView = (BT_ChildNodeView)Activator.CreateInstance(viewType, parent, childNode, graph);
            
            // Setup node selection callback.
            childView.selectedCallback = graph.onChildNodeSelected;

            return childView;
        }

        private static void RegisterNode(BT_Node node, BehaviorTree tree)
        {
            Type nodeType = node.GetType();
            
            // Add the node as an asset to the tree.
            AssetDatabase.AddObjectToAsset(node, tree);
 
            // If undoing after creation, this node will be destroyed
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree Node creation undo");
            
            if (!nodeType.IsSubclassOf(typeof(BT_Decorator))
                && !nodeType.IsSubclassOf(typeof(BT_Service)))
            {
                // Record created node for undoing actions and add it to the behavior tree node list
                Undo.RecordObject(tree, "Undo add nodes");
                tree.nodes.Add(node);
            }
            
            // Save the node asset on the disk
            AssetDatabase.SaveAssets();
        }

        private static void RegisterChildNode(BT_ChildNode child, BT_ParentNode parent)
        {
            if (parent != null)
            {
                Undo.RecordObject(parent, "Undo decorator creation");
                parent.AddChildNode(child);
                EditorUtility.SetDirty(parent);
            }
        }
    }
}

