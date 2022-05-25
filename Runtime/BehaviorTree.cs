using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace BT.Runtime
{
    ///<summary>
    /// Behavior Tree possible states.
    ///</summary>
    public enum EBehaviorTreeState { Running, Success, Failed, Waiting }
    
    ///<summary>
    /// Behavior Tree asset which contains all the data needed from the BehaviorTreeComponent
    /// to make it run.
    /// Behavior Tree execute from left to right and from top to bottom
    ///</summary>
    [CreateAssetMenu(fileName = "New Behavior Tree", menuName = "AI/Behavior Tree")]
    public sealed class BehaviorTree : ScriptableObject
    {

        ///<summary>
        /// The blackboard used by this behavior tree
        ///</summary>
        public Blackboard blackboard;

        ///<summary>
        /// The root node of this behavior tree
        ///</summary>
        [HideInInspector] public BT_RootNode rootNode;
        
        ///<summary>
        /// if true, the beavior tree is going to update each frame, otherwise
        /// it will use a user defined update interval(updateInterval).
        ///</summary>
        public bool canTick = false;

        ///<summary>
        /// The rate at which the behavior tree it's going
        /// to be updated. If canTick is set to true this value will
        /// be ignored.
        ///</summary>
        public float updateInterval = 0.1f;
        
        ///<summary>
        /// The current state of this Behavior Tree.
        ///</summary>
        [HideInInspector] public EBehaviorTreeState treeState;

        ///<summary>
        /// All the Behavior Tree nodes
        ///</summary>
        [HideInInspector] public List<BT_Node> nodes = new List<BT_Node>();
        
        ///<summary>
        /// Clone this behavior tree asset
        ///</summary>
        ///<returns> A copy of this Behavior Tree asset</returns>
        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone() as BT_RootNode;
            tree.blackboard = tree.blackboard.Clone();
            // Initialize behavior tree and blackboard references on each node of the tree
            tree.rootNode.SetBlackboard(tree.blackboard);
            return tree;
        }
    
    #if (UNITY_EDITOR == true)
        
        public BT_Node CreateNode(Type nodeType)
        {
            // Create node and generate GUID
            BT_Node node = ScriptableObject.CreateInstance(nodeType) as BT_Node;
            node.nodeName = nodeType.Name;
            node.guid = GUID.Generate();
            
            if(nodeType == typeof(BT_RootNode))
            {
                rootNode = node as BT_RootNode;
            }

            // Add the node as an asset to the tree.
            AssetDatabase.AddObjectToAsset(node, this);
 
            // If undoing after creation, this node will be destroyed
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree Node creation undos");
            
            if (!nodeType.IsSubclassOf(typeof(BT_Decorator))
                && !nodeType.IsSubclassOf(typeof(BT_Service)))
            {
                // Record created node for undoing actions and add it to the behavior tree node list
                Undo.RecordObject(this, "Undo add nodes");
                nodes.Add(node);
            }
            
            // Save the node asset on the disk
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DestroyNode(BT_Node Node)
        {
            Undo.RegisterCompleteObjectUndo(this, "Behavior tree node removed");
            nodes.Remove(Node);

            // When destroying composite nodes also destroys their decorators and services
            BT_CompositeNode compositeNode = Node as BT_CompositeNode;
            if (compositeNode != null)
            {
                compositeNode.decorators.ForEach(decorator => Undo.DestroyObjectImmediate(decorator));
                compositeNode.services.ForEach(service => Undo.DestroyObjectImmediate(service));
            }

            // When destroying action nodes also destroys their decorators and services
            BT_ActionNode actionNode = Node as BT_ActionNode;
            if (actionNode != null)
            {
                actionNode.decorators.ForEach(decorator => Undo.DestroyObjectImmediate(decorator));
                actionNode.services.ForEach(service => Undo.DestroyObjectImmediate(service));
            }
            
            Undo.DestroyObjectImmediate(Node);
            AssetDatabase.SaveAssets();
        }

        ///<summary>
        /// Add the child node to the specified parent node
        ///</summary>
        public void AddChildToParentNode(BT_Node child, BT_Node parent)
        {
            BT_CompositeNode compositeNode = parent as BT_CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Behavior Tree Composite Node add child");
                compositeNode.childrens.Add(child);
                EditorUtility.SetDirty(compositeNode);
            }

            BT_RootNode rootNode = parent as BT_RootNode;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "Behavior Tree root node set child");
                rootNode.childNode = child;
                EditorUtility.SetDirty(rootNode);
            }
        }

        ///<summary>
        ///Remove the child node from the specified parent node
        ///</summary>
        public void RemoveChildFromParent(BT_Node child, BT_Node parent)
        {
            if (parent.GetType().IsSubclassOf(typeof(BT_CompositeNode)))
            {
                BT_CompositeNode CompositeNode = parent as BT_CompositeNode;
                Undo.RecordObject(CompositeNode, "Behavior Tree Composite Node remove child");
                CompositeNode.childrens.Remove(child);
                EditorUtility.SetDirty(CompositeNode);
            }
        }

        public List<BT_Node> GetChildrenNodes(BT_Node Node)
        {
            List<BT_Node> ChildrenNodes = new List<BT_Node>();

            if (Node.GetType().IsSubclassOf(typeof(BT_CompositeNode)))
            {
                BT_CompositeNode CompositeNode = Node as BT_CompositeNode;
                ChildrenNodes = CompositeNode.childrens;
            }
            return ChildrenNodes;
        }
    #endif
    
    }
}

