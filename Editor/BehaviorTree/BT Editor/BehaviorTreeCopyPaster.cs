using System.Collections.Generic;
using System.Linq;
using BT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BT.Editor
{
    /// <summary>
    /// Used to perform copy/paste operations inside the behavior tree graph.
    /// </summary>
    public sealed class BehaviorTreeCopyPaster
    {
        
        /// <summary>
        /// The rectangle which covers all the selected nodes.
        /// </summary>
        private Rect selectionRectangle;
        
        /// <summary>
        /// Reference to the behavior tree graph view.
        /// </summary>
        private readonly BehaviorTreeGraphView graph;
        
        /// <summary>
        /// Nodes ready to be copied get stored here.
        /// </summary>
        private static List<BT_ParentNodeView> copyCache = new List<BT_ParentNodeView>();
        
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
            // Remove all copied node in favor of the new selected elements.
            copyCache.Clear();
            
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
            copyCache = nodes;
        }
        
        /// <summary>
        /// Paste the copied nodes inside the graph at specified position.
        /// </summary>
        /// <param name="position"> The graph position where you want to paste nodes.</param>
        /// <remarks> Time complexity is Big O(r*n*c) in the worst case, where r is the number of independent roots,
        ///           n is the number of nodes which belongs to a root and c is the number of children of a node.</remarks>
        public void PasteNodes(Vector2 position)
        {
            // Find all the independent roots from the selected nodes.
            var roots = FindRoots();
            foreach (BT_ParentNodeView root in roots)
            {
                // Make a copy of the root.
                BT_ParentNode clonedRoot = NodeFactory.CloneSubtree(root.node, graph.tree);
                
                // Start visiting subtree from one of the root nodes.
                Stack<BT_ParentNode> toVisit = new Stack<BT_ParentNode>();
                toVisit.Push(clonedRoot);
                
                // As long as there are any children to visit, keep iterating.
                while (toVisit.Count > 0)
                {
                    BT_ParentNode node = toVisit.Pop();
                    List<BT_ParentNode> children = node.GetConnectedNodes();
                    
                    // Direction from selection rect center to the node view.
                    Vector2 direction = (node.position - selectionRectangle.center).normalized;
                    // Distance from selection rect center to the node view.
                    float distance = Vector2.Distance(node.position, selectionRectangle.center);
                    
                    // Update the node position. Node will be placed relative to input position(usually mouse position).
                    Vector2 nodePosition = position + direction * distance;
                    Undo.RecordObject(node, "Paste node - Update position");
                    node.position = nodePosition;
                    EditorUtility.SetDirty(node);

                    // Push children inside the to visit stack.
                    children.ForEach(child => toVisit.Push(child));
                }
            }
        }

        /// <summary>
        /// Find all nodes which are considered roots inside the selection.
        /// </summary>
        /// <returns>All the identified roots.</returns>
        /// <remarks>Time complexity of finding roots is Big O(n^2) in the worst case,
        ///          where n is the number of copied nodes.</remarks>
        private List<BT_ParentNodeView> FindRoots()
        {
            var roots = new List<BT_ParentNodeView>();
            // Paste nodes relative to mouse position.
            foreach (BT_ParentNodeView view in copyCache)
            {
                Edge connectionEdge = view.input.connections.FirstOrDefault();
                // Is the node view a root node?
                if ((connectionEdge == null || !copyCache.Contains(connectionEdge.output.node))
                    && view.node.GetType().IsSubclassOf(typeof(BT_CompositeNode)))
                {
                    roots.Add(view);
                }
            }
            return roots;
        }
        
        private int CompareNodes(BT_ParentNodeView left, BT_ParentNodeView right)
        {
            return left.node.position.y < right.node.position.y ? 1 : -1;
        }
    }
}