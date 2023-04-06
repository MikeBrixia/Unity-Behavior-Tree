using System;
using BT.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor
{
    public abstract class BT_ParentNodeView : BT_NodeView
    {
        ///<summary>
        /// Output port of the node
        ///</summary>
        public Port output { get; private set; }

        ///<summary>
        /// Input port of the node
        ///</summary>
        public Port input { get; private set; }
        
        ///<summary>
        /// The position of the mouse.
        ///</summary>
        private Vector2 mousePosition;
        
        protected BT_ParentNodeView(BT_ParentNode node, BehaviorTreeGraphView graph, string path) : base(node, graph, path)
        {
            // Set node position in the graph to where the user has clicked
            // to open the contextual menu
            Rect rect = this.contentRect;
            rect.position = this.node.position;
            SetPosition(rect);
            
            Draw();
        }
        
        ///<summary>
        /// Create input port for this node view
        ///</summary>
        private void CreateInputPort()
        {
            if (node.GetType() != typeof(BT_RootNode))
            {
                // Create input port
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, null);
                input.style.flexDirection = FlexDirection.Column;
                // Add ports to their respective container
                inputContainer.Add(input);
            }
        }
        
        ///<summary>
        /// Create output port for this node view
        ///</summary>
        private void CreateOutputPort()
        {
            if (!node.GetType().IsSubclassOf(typeof(BT_ActionNode)))
            {
                Port.Capacity PortCapacity = node.GetType().IsSubclassOf(typeof(BT_CompositeNode)) ? Port.Capacity.Multi : Port.Capacity.Single;

                // Create output port
                output = InstantiatePort(Orientation.Vertical, Direction.Output, PortCapacity, null);
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }
        
        ///<summary>
        /// Draw basic node layout
        ///</summary>
        public virtual void Draw()
        {
            // Initialize node ports
            CreateInputPort();
            CreateOutputPort();
            RefreshExpandedState();
        }
        
        ///<summary>
        /// Set the position of this node view.
        ///</summary>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Node position");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
        
        ///<summary>
        /// Show or hide node border.
        ///</summary>
        public void SortChildrenNodes()
        {
            BT_CompositeNode compositeNode = node as BT_CompositeNode;
            if (compositeNode != null)
            {
                compositeNode.childrens.Sort(SortByPosition);
            }
        }
        
        private int SortByPosition(BT_Node left, BT_Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }
        
        ///<summary>
        /// Create contextual menu to handle node visual element creation.
        ///</summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            
            // Is the node wrapped inside this view a root node?
            if (node.GetType() != typeof(BT_RootNode))
            {
                // Search for all Child node derived types
                var childTypes = TypeCache.GetTypesDerivedFrom<BT_ChildNode>();
                
                // For each child node type in the project create an action
                // which allows developers to create child nodes and attach them
                // to this node view.
                foreach (Type type in childTypes)
                {
                    if (type.BaseType != null & node != null & !type.IsAbstract)
                    {
                        string actionName = type.BaseType.Name + "/" + type.Name;
                        // Remove the BT_ prefix from the node type name
                        actionName = actionName.Remove(0, 3);
                        evt.menu.AppendAction(actionName, (a) => 
                            graph.CreateChildNode(type, this));
                    }
                }
            }
        }
    }
}