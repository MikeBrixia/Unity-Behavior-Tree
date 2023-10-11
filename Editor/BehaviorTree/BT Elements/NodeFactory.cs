using System;
using System.Collections.Generic;
using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEngine;

namespace BT
{
    /// <summary>
    /// Factory responsible of creating, destroying and cloning BT nodes and views.
    /// When performing the operations listed above, inside the editor, this class
    /// should be use to ensure data consistency inside the behavior tree.
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
                
                // Register the child node inside the tree.
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
            
            // Create a node view of the type associated with the node type.
            BT_ParentNodeView parentNodeView = CreateNodeView<BT_ParentNodeView>(node, node, graph);
            
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
            
            // Create view using reflection and initialize it.
            BT_ChildNodeView childView = CreateNodeView<BT_ChildNodeView>(childNode, parent, childNode, graph);
            childView.selectedCallback = graph.onChildNodeSelected;
            return childView;
        }
        
        private static T CreateNodeView<T>(BT_Node node, params object[] args)
        {
            ConfigData config = BTInstaller.btConfig;
            
            // Check if this node type has an associated view.
            string viewTypeName;
            Type nodeType = node.GetType();
            Type nodeBaseType = nodeType.BaseType;
            
            bool hasView = config.nodeViews.TryGetValue(nodeType.ToString(), out viewTypeName);
            if (!hasView)
            {
                config.defaultNodeViews.TryGetValue(nodeBaseType.ToString(), out viewTypeName);
            }
            
            // Create view using reflection and initialize it.
            T childView = (T) Activator.CreateInstance(Type.GetType(viewTypeName), args);
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
        
        /// <summary>
        /// Clone a subtree formed by the supplied root.
        /// </summary>
        /// <param name="root"> The root of the subtree we're going to clone. </param>
        /// <param name="tree"> Reference to the behavior tree asset. </param>
        /// <returns> The root of the cloned sub-tree. </returns>
        public static BT_ParentNode CloneSubtree(BT_ParentNode root, BehaviorTree tree)
        {
            // Create a queue for all the nodes which needs to be cloned and push the subtree root
            // inside it.
            var toClone = new Queue<KeyValuePair<BT_ParentNode, BT_ParentNode>>();
            var currentParentChildPair = new KeyValuePair<BT_ParentNode, BT_ParentNode>(null, root);
            toClone.Enqueue(currentParentChildPair);
            
            BT_ParentNode clonedRoot = null;
            int count = 0;
            while (toClone.Count > 0)
            {
                // Get the first node of the queue and clone it.
                currentParentChildPair = toClone.Dequeue();
                BT_ParentNode node = currentParentChildPair.Value;
                BT_ParentNode parent = currentParentChildPair.Key;
                
                // Clone the node.
                node = CloneParentNode(node, tree);
                
                // does the node have parent?
                if (parent != null)
                {
                    // If true, connect the parent node to the current cloned node.
                    Undo.RecordObject(parent, "Cloning - Record node connection");
                    parent.ConnectNode(node);
                    EditorUtility.SetDirty(parent);
                }
                
                // Push the children to the clone queue, they need to be cloned.
                List<BT_ParentNode> children = node.GetConnectedNodes();
                foreach (BT_ParentNode child in children)
                {
                    var childParentPair = new KeyValuePair<BT_ParentNode, BT_ParentNode>(node, child);
                    toClone.Enqueue(childParentPair);
                }
                
                // Remove all references to source children. The behavior tree graph
                // will automatically replace source children with the cloned ones.
                children.Clear();
                
                // Keep track of the root of the subtree, it will be the
                // return value of the function.
                count++;
                if (count == 1)
                {
                    clonedRoot = node;
                }
            }
            return clonedRoot;
        }
       
        /// <summary>
        /// Clone a parent node with all the children attached to it.
        /// </summary>
        /// <param name="node"> The parent node to clone. </param>
        /// <param name="tree"> Reference to the behavior tree asset. </param>
        /// <returns> The cloned parent node with all it's attached children cloned as well. </returns>
        public static BT_ParentNode CloneParentNode(BT_ParentNode node, BehaviorTree tree)
        {
            // Clone/copy the requested node.
            BT_ParentNode copiedNode = (BT_ParentNode) CloneNode(node);
            
            // Register node inside the behavior tree asset.
            RegisterNode(copiedNode, tree);
            
            List<BT_Decorator> decorators = copiedNode.GetChildNodes<BT_Decorator>();
            int decoratorsCount = decorators.Count;
            for (int i = 0; i < decoratorsCount; i++)
            {
                // Clone decorator node.
                BT_Decorator decorator = decorators[0];
                CloneChildNode(decorator, copiedNode, tree);
                
                // Remove source node.
                decorators.RemoveAt(0);
            }

            List<BT_Service> services = copiedNode.GetChildNodes<BT_Service>();
            int servicesCount = services.Count;
            for (int i = 0; i < servicesCount; i++)
            {
                // Clone service node.
                BT_Service service = services[0];
                CloneChildNode(service, copiedNode, tree);
                
                // Remove source node.
                services.RemoveAt(0);
            }
            
            return copiedNode;
        }
        
        /// <summary>
        /// Clone the child node and register it upon is parent.
        /// </summary>
        /// <param name="node"> The child node to clone. </param>
        /// <param name="parent"> The parent to which the child node should be attached. </param>
        /// <param name="tree"> Reference to the behavior tree asset. </param>
        /// <returns> The cloned child node. </returns>
        public static BT_ChildNode CloneChildNode(BT_ChildNode node, BT_ParentNode parent, BehaviorTree tree)
        {
            // Clone/copy the requested node.
            BT_ChildNode copiedNode = (BT_ChildNode) CloneNode(node);
            
            // Register node inside the behavior tree asset.
            RegisterNode(copiedNode, tree);
            
            // Register node inside it's parent node.
            RegisterChildNode(copiedNode, parent);

            return copiedNode;
        }
        
        /// <summary>
        /// Clone a behavior tree node.
        /// </summary>
        /// <param name="node"> The behavior tree node to clone. </param>
        /// <returns> The cloned behavior tree node. </returns>
        private static BT_Node CloneNode(BT_Node node)
        {
            // Clone/copy the requested node.
            BT_Node copiedNode = ScriptableObject.Instantiate(node);
            copiedNode.name = "";
            copiedNode.guid = GUID.Generate();
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
        
        public static void RegisterNode(BT_Node node, BehaviorTree tree)
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
        
        public static void RegisterChildNode(BT_ChildNode child, BT_ParentNode parent)
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

