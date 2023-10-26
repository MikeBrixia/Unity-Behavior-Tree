using System;
using System.Collections.Generic;
using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEngine;

namespace BT.Editor
{
    /// <summary>
    /// Factory responsible of creating, destroying and cloning BT nodes and views.
    /// When performing the operations listed above, inside the editor, this class
    /// should be use to ensure data consistency inside the behavior tree.
    /// </summary>
    public static class NodeFactory
    {
        /// <summary>
        /// Create a brand new BT childNode and initialize it inside the
        /// behavior tree asset.
        /// </summary>
        /// <param name="nodeType"> The type of the BT childNode; reflection will
        ///                         use this type to create the childNode</param>
        /// <param name="tree"> The behavior tree asset inside which the childNode
        ///                     will be created.</param>
        /// <returns>The created childNode. </returns>
        public static BT_Node CreateNode(Type nodeType, BehaviorTree tree)
        {
            // Create childNode and generate GUID
            BT_Node node = ScriptableObject.CreateInstance(nodeType) as BT_Node;
            if (node != null)
            {
                node.nodeTypeName = nodeType.Name;
                node.guid = GUID.Generate();
                node.SetBlackboard(tree.blackboard);
                
                // If the new childNode is a root childNode, override
                // the old root childNode with the new one.
                if(nodeType == typeof(BT_RootNode))
                {
                    tree.rootNode = node as BT_RootNode;
                }
            
                // Register the childNode in the behavior tree.
                RegisterNode(node, tree);
            }
            return node;
        }
        
        /// <summary>
        /// Create a new childNode of type T and initialize it inside the behavior
        /// tree asset.
        /// </summary>
        /// <param name="tree"> The behavior tree asset inside which the childNode
        ///                     will be created.</param>
        /// <typeparam name="T"> The type of the BT childNode; reflection will
        ///                      use this type to create the childNode</typeparam>
        /// <returns> The created childNode type casted to T</returns>
        public static T CreateNode<T>(BehaviorTree tree) where T : BT_Node
        {
            // The type of the childNode we want to create.
            Type nodeType = typeof(T);
            
            // Create childNode and generate GUID
            T node = CreateNode(nodeType, tree) as T;
            return node;
        }
        
        /// <summary>
        /// Create a child childNode and attach it to it's parent.
        /// </summary>
        /// <param name="nodeType"> The type of the BT childNode; reflection will
        ///                         use this type to create the childNode </param>
        /// <param name="parent"> The parent childNode to which the child will be attached</param>
        /// <param name="tree"> The behavior tree asset inside which the childNode
        ///                     will be created. </param>
        /// <returns> The created child childNode. </returns>
        public static BT_Node CreateChildNode(Type nodeType, BT_ParentNode parent, BehaviorTree tree)
        {
            // Create childNode and generate GUID
            BT_ChildNode node = ScriptableObject.CreateInstance(nodeType) as BT_ChildNode;
            if (node != null)
            {
                // Initialize childNode parameters.
                node.nodeTypeName = nodeType.Name;
                node.guid = GUID.Generate();
                node.SetBlackboard(tree.blackboard);
                
                // Register the child childNode inside the tree.
                RegisterNode(node, tree);
                
                // Register child childNode inside parent childNode
                RegisterChildNode(node, parent);
            }
            return node;
        }
        
        /// <summary>
        /// Create a child childNode and attach it to it's parent.
        /// </summary>
        /// <param name="parent"> The parent childNode to which the child will be attached </param>
        /// <param name="tree"> The behavior tree asset inside which the childNode
        ///                     will be created. </param>
        /// <typeparam name="TChildType"> The type of the BT childNode; reflection will
        ///                              use this type to create the childNode </typeparam>
        /// <typeparam name="TParentType"> The type of the parent childNode. </typeparam>
        /// <returns></returns>
        private static TChildType CreateChildNode<TChildType, TParentType>(TParentType parent, BehaviorTree tree) where TChildType : BT_ChildNode
                                                                                                                  where TParentType : BT_ParentNode
        {
            // Create a childNode of TChildType.
            Type nodeType = typeof(TChildType);
            TChildType node = CreateChildNode(nodeType, parent, tree) as TChildType;
            return node;
        }
        
        ///<summary>
        /// Create a brand new childNode view
        ///</summary>
        ///<param name="childNode"> The childNode which will be wrapped inside the new childNode view </param>
        /// <returns> The created childNode view type casted to T</returns>
        public static T CreateNodeView<T>(BT_Node childNode, BehaviorTreeGraphView graph) where T : BT_ParentNodeView
        {
            // When guid is invalid, generate a brand new one
            if (childNode.guid.Empty())
            {
                childNode.guid = GUID.Generate();
            }
            
            // Create childNode view of type T.
            T nodeView = (T)Activator.CreateInstance(typeof(T),childNode, graph);
            
            // Create all attached and supported nodes for this parent childNode view.
            nodeView.CreateChildViews();
            
            // Setup childNode selection callback.
            nodeView.onNodeSelected = graph.onNodeSelected;
            
            return nodeView;
        }

        /// <summary>
        ///  Create a brand new childNode view
        /// </summary>
        /// <param name="childNode"> The childNode which will be wrapped inside the new childNode view </param>
        /// <param name="graph"> The graph in which the tree is currently displayed. </param>
        /// /// <returns> The created childNode view. </returns>
        public static BT_ParentNodeView CreateNodeView(BT_Node childNode, BehaviorTreeGraphView graph)
        {
            // When guid is invalid, generate a brand new one
            if (childNode.guid.Empty())
            {
                childNode.guid = GUID.Generate();
            }
            
            // Create a childNode view of the type associated with the childNode type.
            BT_ParentNodeView parentNodeView = CreateNodeView<BT_ParentNodeView>(childNode, childNode, graph);
            
            // Create all attached and supported nodes for this parent childNode view.
            parentNodeView.CreateChildViews();
            
            // Setup childNode selection callback.
            parentNodeView.onNodeSelected = graph.onNodeSelected;
            
            return parentNodeView;
        }
        
        /// <summary>
        /// Create a child childNode view attached to it's parent view.
        /// </summary>
        /// <param name="parent">The parent view in which the child view will be contained </param>
        /// <param name="childNode"> The child childNode from which we will create the view. </param>
        /// <param name="graph"> The graph in which the view will be placed. </param>
        /// <returns> The created child view. </returns>
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
        
        /// <summary>
        /// Create a child childNode view attached to it's parent view.
        /// </summary>
        /// <param name="childNode"> The child childNode from which we will create the view. </param>
        /// <param name="args"> input arguments for creating the child view. </param>
        /// <returns> The created child view type casted T. </returns>
        private static T CreateNodeView<T>(BT_Node childNode, params object[] args)
        {
            // The current behavior tree config.
            ConfigData config = BTInstaller.btConfig;
            
            // Check if this childNode type has an associated view.
            string viewTypeName;
            Type nodeType = childNode.GetType();
            Type nodeBaseType = nodeType.BaseType;
            
            bool hasView = config.nodeViews.TryGetValue(nodeType.ToString(), out viewTypeName);
            // If we don't find a type-specific view, fallback to base type childNode view.
            if (!hasView)
            {
                config.defaultNodeViews.TryGetValue(nodeBaseType.ToString(), out viewTypeName);
            }
            
            // Create view using reflection and initialize it.
            T childView = (T) Activator.CreateInstance(Type.GetType(viewTypeName), args);
            return childView;
        }
        
        /// <summary>
        /// Destroy the given parent node and remove it from behavior tree asset.
        /// </summary>
        /// <param name="node"> The node to destroy.</param>
        /// <param name="tree"> The asset from which the node will be removed. </param>
        public static void DestroyParentNode(BT_ParentNode node, BehaviorTree tree)
        {
            // Remove childNode from the tree.
            Undo.RegisterCompleteObjectUndo(tree, "Behavior tree childNode removed");
            tree.nodes.Remove(node);
            
            // Save childNode state by registering an undo/redo action and then destroy it.
            Undo.DestroyObjectImmediate(node);
            
            // Destroy all children nodes.
            node.DestroyChildrenNodes();
            
            // Save operations to disk.
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Destroy the given child node and remove it from it's parent and
        /// behavior tree asset.
        /// </summary>
        /// <param name="parent"> The parent of the child node</param>
        /// <param name="child"> The node to remove. </param>
        /// <param name="tree">The asset from which the node will be removed.</param>
        public static void DestroyChildNode(BT_ParentNode parent, BT_ChildNode child, BehaviorTree tree)
        {
            // Remove childNode from the tree.
            Undo.RegisterCompleteObjectUndo(tree, "Behavior Tree child childNode removed");
            tree.nodes.Remove(child);
            
            // Remove child from parent childNode.
            Undo.RecordObject(parent, "Behavior Tree child childNode removed");
            parent.DestroyChild(child);
            
            // Destroy childNode.
            Undo.DestroyObjectImmediate(child);
            
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
                // Get the first childNode of the queue and clone it.
                currentParentChildPair = toClone.Dequeue();
                BT_ParentNode node = currentParentChildPair.Value;
                BT_ParentNode parent = currentParentChildPair.Key;
                
                // Clone the childNode.
                node = CloneParentNode(node, tree);
                
                // does the childNode have parent?
                if (parent != null)
                {
                    // If true, connect the parent childNode to the current cloned childNode.
                    Undo.RecordObject(parent, "Cloning - Record childNode connection");
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
        /// Clone a parent childNode with all the children attached to it.
        /// </summary>
        /// <param name="node"> The parent childNode to clone. </param>
        /// <param name="tree"> Reference to the behavior tree asset. </param>
        /// <returns> The cloned parent childNode with all it's attached children cloned as well. </returns>
        public static BT_ParentNode CloneParentNode(BT_ParentNode node, BehaviorTree tree)
        {
            // Clone/copy the requested childNode.
            BT_ParentNode copiedNode = (BT_ParentNode) CloneNode(node);
            
            // Register childNode inside the behavior tree asset.
            RegisterNode(copiedNode, tree);
            
            List<BT_Decorator> decorators = copiedNode.GetChildNodes<BT_Decorator>();
            int decoratorsCount = decorators.Count;
            for (int i = 0; i < decoratorsCount; i++)
            {
                // Clone decorator childNode.
                BT_Decorator decorator = decorators[0];
                CloneChildNode(decorator, copiedNode, tree);
                
                // Remove source childNode.
                decorators.RemoveAt(0);
            }

            List<BT_Service> services = copiedNode.GetChildNodes<BT_Service>();
            int servicesCount = services.Count;
            for (int i = 0; i < servicesCount; i++)
            {
                // Clone service childNode.
                BT_Service service = services[0];
                CloneChildNode(service, copiedNode, tree);
                
                // Remove source childNode.
                services.RemoveAt(0);
            }
            
            return copiedNode;
        }
        
        /// <summary>
        /// Clone the child childNode and register it upon is parent.
        /// </summary>
        /// <param name="node"> The child childNode to clone. </param>
        /// <param name="parent"> The parent to which the child childNode should be attached. </param>
        /// <param name="tree"> Reference to the behavior tree asset. </param>
        /// <returns> The cloned child childNode. </returns>
        public static BT_ChildNode CloneChildNode(BT_ChildNode node, BT_ParentNode parent, BehaviorTree tree)
        {
            // Clone/copy the requested childNode.
            BT_ChildNode copiedNode = (BT_ChildNode) CloneNode(node);
            
            // Register childNode inside the behavior tree asset.
            RegisterNode(copiedNode, tree);
            
            // Register childNode inside it's parent childNode.
            RegisterChildNode(copiedNode, parent);

            return copiedNode;
        }
        
        /// <summary>
        /// Clone a behavior tree childNode.
        /// </summary>
        /// <param name="node"> The behavior tree childNode to clone. </param>
        /// <returns> The cloned behavior tree childNode. </returns>
        private static BT_Node CloneNode(BT_Node node)
        {
            // Clone/copy the requested childNode.
            BT_Node copiedNode = ScriptableObject.Instantiate(node);
            copiedNode.name = "";
            copiedNode.guid = GUID.Generate();
            return copiedNode;
        }
        
        /// <summary>
        /// Register the bt node inside a behavior tree asset.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="tree">The asset to which the node will be added. </param>
        public static void RegisterNode(BT_Node node, BehaviorTree tree)
        {
            Type nodeType = node.GetType();
            
            // Add the childNode as an asset to the tree.
            AssetDatabase.AddObjectToAsset(node, tree);
 
            // If undoing after creation, this childNode will be destroyed
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree Node creation undo");
            
            if (!nodeType.IsSubclassOf(typeof(BT_Decorator))
                && !nodeType.IsSubclassOf(typeof(BT_Service)))
            {
                // Record created childNode for undoing actions and add it to the behavior tree childNode list
                Undo.RegisterCompleteObjectUndo(tree, "Undo add nodes");
                tree.nodes.Add(node);
                EditorUtility.SetDirty(tree);
            }
            
            // Save the childNode asset on the disk
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Register the bt child node inside the parent.
        /// </summary>
        /// <param name="child">The node to add.</param>
        /// <param name="parent">The parent in which the node will be registered. </param>
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

