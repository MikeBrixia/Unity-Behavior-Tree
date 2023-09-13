using System.Collections.Generic;
using System.Linq;
using BT.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BT.Editor
{
    
    /// <summary>
    /// Class responsible of handling behavior tree debug features..
    /// </summary>
    public class BehaviorTreeDebugger
    {
        /// <summary>
        /// Reference to the behavior tree editor who owns this debugger.
        /// </summary>
        private BehaviorTreeEditor editor;

        public BehaviorTreeDebugger(BehaviorTreeEditor editor)
        {
            this.editor = editor;
        }
        
        /// <summary>
        /// Debug the whole behavior tree graph editor.
        /// </summary>
        /// <param name="graph"> The behavior tree graph </param>
        /// <param name="tree">The tree asset to debug. </param>
        public void DebugGraphEditor(BehaviorTreeGraphView graph, BehaviorTree tree)
        {
            // Reset edges appearance.
            foreach (Edge edge in graph.edges)
            {
                edge.edgeControl.edgeWidth = 2;
                edge.edgeControl.inputColor = Color.white;
            }
            
            // Highlight all the edges connecting nodes which
            // are been executed.
            DebugExecutionPath(graph, tree);
        }
        
        /// <summary>
        /// Debug all the nodes executed during a debugger update.
        /// </summary>
        /// <param name="nodeView"> The node view to debug. </param>
        private void DebugExecutedNodes(BT_ParentNodeView nodeView)
        {
            
        }
        
        /// <summary>
        /// Debug the current behavior tree execution path by highlighting the path edges from
        /// root to the executed nodes
        /// </summary>
        /// <param name="graph"> The graph in which we're going to debug. </param>
        /// <param name="behaviorTree"> The behavior tree asset to debug. </param>
        /// <remarks> Time complexity of debugging execution paths is Big O(h), where
        ///           "h" is the height of the behavior tree. </remarks>
        private void DebugExecutionPath(BehaviorTreeGraphView graph, BehaviorTree behaviorTree)
        {
            // Ensure that the selected behavior tree asset is a clone of the currently
            // inspected behavior tree.
            if (behaviorTree.IsCloneOf(graph.tree) && behaviorTree.rootNode != null)
            {
                // Initialize visit queue with the root node as the first node to visit.
                Queue<BT_ParentNode> toVisit = new Queue<BT_ParentNode>();
                toVisit.Enqueue(behaviorTree.rootNode);
                
                // Keep visiting as long as there are nodes to visit.
                while (toVisit.Count != 0)
                {
                    BT_ParentNode currentNode = toVisit.Dequeue();
                    List<BT_ParentNode> children = currentNode.GetConnectedNodes();
                    
                    // Does the current node have any child?
                    if (children.Count > 0)
                    {
                        // Find the node view associated with the currently executed child.
                        BT_ParentNode child = children[currentNode.executionIndex];
                        if (child.state != ENodeState.Failed)
                        {
                            BT_ParentNodeView currentNodeView = graph.FindNodeView(child);
                            // Get the edge which connects the child to the parent.
                            Edge connectionEdge = currentNodeView.input.connections.First();

                            // Add the child to the visit queue.
                            toVisit.Enqueue(child);
                            
                            // Highlight the edge connecting the node to it's parent.
                            HighlightEdge(connectionEdge);
                            // Debug the node 
                            DebugExecutedNodes(currentNodeView);
                        }
                    }
                }
            }
        }

        private void HighlightEdge(Edge edge)
        {
            edge.edgeControl.edgeWidth = 10;
            edge.edgeControl.inputColor = Color.yellow;
        }
    }
}