using System.Collections.Generic;
using System.Linq;
using BT.Runtime;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace BT.Editor
{
    /// <summary>
    /// Used to perform copy/paste operations inside the behavior tree graph.
    /// </summary>
    public sealed class BehaviorTreeCopyPaster
    {
        
        /// <summary>
        /// The 2D rectangle which contains all the copied nodes.
        /// </summary>
        private Rect selectionRectangle;
        
        /// <summary>
        /// Reference to the behavior tree graph view.
        /// </summary>
        private readonly BehaviorTreeGraphView graph;
        
        /// <summary>
        /// A cache for all the identified roots inside
        /// the latest copy operation.
        /// </summary>
        private List<BT_ParentNode> copiedRoots;
        
        /// <summary>
        /// Nodes ready to be copied get stored here.
        /// </summary>
        private static List<BT_ParentNode> copyCache = new List<BT_ParentNode>();
        
        public BehaviorTreeCopyPaster(BehaviorTreeGraphView graph)
        {
            this.graph = graph;
        }
        
        /// <summary>
        /// Copy all the nodes to the copy cache. Call paste nodes
        /// when you want to cloned them and place them inside the graph editor.
        /// </summary>
        /// <param name="nodes"> The nodes to copy</param>
        /// <remarks> Time complexity is mainly carried by .NET sort,
        ///           which in worst case is Big O(n* log n)</remarks>
        public void CopyNodes(List<BT_ParentNodeView> nodes)
        {
            // Remove all copied node in favor of the new ones.
            copyCache.Clear();
            
            // Remove root node from copied nodes.
            nodes.RemoveAll(node => node.node is BT_RootNode);
            
            // Find the selected nodes which are the most left and right nodes in the graph.
            nodes.Sort();
            float xMin = nodes[0].node.position.x;
            float xMax = nodes[^1].node.position.x;
            
            // Find the selected nodes which are the most up and down nodes in the graph.
            nodes.Sort(CompareNodes);
            float yMin = nodes[0].node.position.y;
            float yMax = nodes[^1].node.position.y;
            
            // Create the selected nodes rectangle.
            selectionRectangle = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            
            // Find all the independent roots from the selected nodes.
            copiedRoots = FindRoots(nodes);
            foreach (BT_ParentNodeView nodeView in nodes)
            {
                copyCache.Add(nodeView.node);
            }
        }
        
        /// <summary>
        /// Paste the copied nodes inside the graph at specified position.
        /// </summary>
        /// <param name="position"> The graph position where you want to paste nodes.</param>
        public void PasteNodes(Vector2 position)
        {
            // Begin cloning process for each root.
            foreach (BT_ParentNode root in copiedRoots)
            {
                // Is the root an action node?
                if (root is BT_ActionNode actionNode)
                {
                    // If true, then clone only the action node. Action nodes
                    // cannot have any children, therefore cloning an entire
                    // subtree would not make any sense.
                    BT_ParentNode clonedAction = NodeFactory.CloneParentNode(actionNode, graph.tree);
                    // Finally, move it to paste position.
                    MoveAtPasteLocation(clonedAction, position);
                }
                else
                {
                    // Otherwise, starting from this root, clone the
                    // entire subtree.
                    CloneCopiedSubtree(root, position);
                }
            }
        }

        /// <summary>
        /// Find all nodes which are considered roots inside the selection.
        /// </summary>
        /// <returns>All the identified roots.</returns>
        /// <remarks>Time complexity of finding roots is Big O(n^2) in the worst case,
        ///          where n is the number of copied nodes.</remarks>
        private List<BT_ParentNode> FindRoots(List<BT_ParentNodeView> nodes)
        {
            var roots = new List<BT_ParentNode>();
            // Paste nodes relative to mouse position.
            foreach (BT_ParentNodeView view in nodes)
            {
                Edge connectionEdge = view.input.connections.FirstOrDefault();
                // Is the node view a composite or action root?
                if (connectionEdge == null || !nodes.Contains(connectionEdge.output.node))
                {
                    roots.Add(view.node);
                }
            }
            return roots;
        }
        
        private BT_ParentNode CloneCopiedSubtree(BT_ParentNode root, Vector2 position)
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
                
                // Clone the node and move it to the paste location.
                node = NodeFactory.CloneParentNode(node, graph.tree);
                MoveAtPasteLocation(node, position);
                
                // does the node have parent?
                if (parent != null)
                {
                    // If true, connect the parent node to the current cloned node.
                    Undo.RecordObject(parent, "Cloning - Record node connection");
                    parent.ConnectNode(node);
                    EditorUtility.SetDirty(parent);
                }
                
                // Push all copied children inside the clone queue.
                List<BT_ParentNode> children = node.GetConnectedNodes();
                foreach (BT_ParentNode child in children)
                {
                    // Is the node been copied inside the copy cache?
                    if (copyCache.Contains(child))
                    {
                        // If true, proceed and add it the clone queue.
                        var childParentPair = new KeyValuePair<BT_ParentNode, BT_ParentNode>(node, child);
                        toClone.Enqueue(childParentPair);
                    }
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

        private void MoveAtPasteLocation(BT_ParentNode node, Vector2 position)
        {
            Undo.RecordObject(node, "Paste node - Update position");
            // Update the node position. Node will be placed relative to input position(usually mouse position).
            node.position = position + (node.position - selectionRectangle.center);
            EditorUtility.SetDirty(node);
        }
        
        private int CompareNodes(BT_ParentNodeView left, BT_ParentNodeView right)
        {
            return left.node.position.y < right.node.position.y ? 1 : -1;
        }
    }
}