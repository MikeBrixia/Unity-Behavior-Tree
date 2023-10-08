using System;
using System.Collections.Generic;
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
                node.SetBlackboard(tree.blackboard);
                
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
            T node = CreateNode(nodeType, tree) as T;
            return node;
        }

        public static BT_Node CreateChildNode(Type nodeType, BT_ParentNode parent, BehaviorTree tree)
        {
            // Create node and generate GUID
            BT_ChildNode node = ScriptableObject.CreateInstance(nodeType) as BT_ChildNode;
            if (node != null)
            {
                // Initialize node parameters.
                node.nodeTypeName = nodeType.Name;
                node.guid = GUID.Generate();
                node.SetBlackboard(tree.blackboard);
                
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
            // Create a node of TChildType.
            Type nodeType = typeof(TChildType);
            TChildType node = CreateChildNode(nodeType, parent, tree) as TChildType;
            return node;
        }
        
        ///<summary>
        /// Create a brand new node view
        ///</summary>
        ///<param name="node"> The node which will be wrapped inside the new node view </param>
        public static T CreateNodeView<T>(BT_Node node, BehaviorTreeGraphView graph) where T : BT_ParentNodeView
        {
            // When guid is invalid, generate a brand new one
            if (node.guid.Empty())
            {
                node.guid = GUID.Generate();
            }
            
            // Create node view of type T.
            T nodeView = (T)Activator.CreateInstance(typeof(T),node, graph);
            
            // Create all attached and supported nodes for this parent node view.
            nodeView.CreateChildViews();
            
            // Setup node selection callback.
            nodeView.onNodeSelected = graph.onNodeSelected;
            
            return nodeView;
        }

        /// <summary>
        ///  Create a brand new node view
        /// </summary>
        /// <param name="node"> The node which will be wrapped inside the new node view </param>
        /// <param name="graph"> The graph in which the tree is currently displayed. </param>
        public static BT_ParentNodeView CreateNodeView(BT_Node node, BehaviorTreeGraphView graph)
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
            BT_ParentNodeView parentNodeView = (BT_ParentNodeView)Activator.CreateInstance(viewType,node, graph);
            
            // Create all attached and supported nodes for this parent node view.
            parentNodeView.CreateChildViews();
            
            // Setup node selection callback.
            parentNodeView.onNodeSelected = graph.onNodeSelected;
            
            return parentNodeView;
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
            
            childView.selectedCallback = graph.onChildNodeSelected;
            return childView;
        }

        public static void DestroyParentNode(BT_ParentNode node, BehaviorTree tree)
        {
            // Remove node from the tree.
            Undo.RegisterCompleteObjectUndo(tree, "Behavior tree node removed");
            tree.nodes.Remove(node);
            
            // Save node state by registering an undo/redo action and then destroy it.
            Undo.DestroyObjectImmediate(node);
            
            // Destroy all children nodes.
            node.DestroyChildrenNodes();
            
            // Save operations to disk.
            AssetDatabase.SaveAssets();
        }
        
        public static BT_Node CloneNode(BT_Node node, BehaviorTree tree, bool recursive = true)
        {
            BT_Node copiedNode = recursive? node.Clone() : ScriptableObject.Instantiate(node);
            copiedNode.name = "";
            copiedNode.guid = GUID.Generate();

            if (recursive)
            {
                Stack<BT_Node> toVisit = new Stack<BT_Node>();
                toVisit.Push(copiedNode);
                
                RegisterNode(copiedNode, tree);
                while (toVisit.Count > 0)
                {
                    BT_ParentNode currentNode = (BT_ParentNode) toVisit.Pop();
                    List<BT_ParentNode> children = currentNode.GetConnectedNodes();
                    
                    foreach(var a in children)
                        Debug.Log(a);
                    children.ForEach(child => RegisterNode(child, tree));
                }
            }

            return copiedNode;
        }
        
        public static void DestroyChildNode(BT_ParentNode parent, BT_ChildNode child, BehaviorTree tree)
        {
            // Remove node from the tree.
            Undo.RegisterCompleteObjectUndo(tree, "Behavior Tree child node removed");
            tree.nodes.Remove(child);
            
            // Remove child from parent node.
            Undo.RecordObject(parent, "Behavior Tree child node removed");
            parent.DestroyChild(child);
            
            // Destroy node.
            Undo.DestroyObjectImmediate(child);
            
            // Save operations to disk.
            AssetDatabase.SaveAssets();
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
                Undo.RegisterCompleteObjectUndo(tree, "Undo add nodes");
                tree.nodes.Add(node);
                EditorUtility.SetDirty(tree);
            }
            
            // Save the node asset on the disk
            AssetDatabase.SaveAssets();
        }
        
        private static void RegisterChildNode(BT_ChildNode child, BT_ParentNode parent)
        {
            if (parent != null)
            {
                Undo.RegisterCompleteObjectUndo(parent, "Undo add nodes");
                parent.AddChildNode(child);
                EditorUtility.SetDirty(parent);
            }
        }
    }
}

