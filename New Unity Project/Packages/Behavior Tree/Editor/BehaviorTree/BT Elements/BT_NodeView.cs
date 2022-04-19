using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.EventSystems;
using UnityEditor.UIElements;

namespace BT
{

    public class BT_NodeView : Node
    {

        ///<summary>
        ///Reference to the node encapsulated inside this node view, this value is going
        ///to contain the actual instructions of the node
        ///</summary>
        public BT_Node node { get; private set; }

        ///<summary>
        /// decorator views containted inside this node view
        ///</summary>
        public List<BT_DecoratorView> decoratorViews { get; private set; }
        
        ///<summary>
        /// service views containted inside this node view
        ///</summary>
        public List<BT_ServiceView> serviceViews { get; private set; }
        
        ///<summary>
        /// Called when this node view gets selected by the user
        ///</summary>
        public Action<BT_NodeView> OnNodeSelected;

        ///<summary>
        /// Output port of the node
        ///</summary>
        public Port output { get; private set; }

        ///<summary>
        /// Input port of the node
        ///</summary>
        public Port input { get; private set; }

        public VisualElement decoratorsContainer { get; private set;}
        public VisualElement serviceContainer { get; private set; }
        
        private GUID guid;

        ///<summary>
        /// The graph which owns this node
        ///</summary>
        public BehaviorTreeGraphView behaviorTreeGraph { get; private set; }
        private Vector2 mousePosition;

        private Label nodeNameLabel;
        private Label nodeDescriptionLabel;
        private VisualElement titleElement;
        private VisualElement nodeBorder;
        
        public BT_NodeView(BT_Node node, BehaviorTreeGraphView graph) : base("Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/NodeView.uxml")
        {
            this.viewDataKey = node.guid.ToString();
            this.node = node;
            this.behaviorTreeGraph = graph;
            InitializeUIElements();
            
            // Set node position in the graph to where the user has clicked
            // to open the contextual menu
            Rect rect = this.contentRect;
            rect.position = this.node.position;
            SetPosition(rect);

            // Register mouse callbacks
            EventCallback<MouseEnterEvent> mouseEnterEvent = OnMouseEnter;
            RegisterCallback<MouseEnterEvent>(mouseEnterEvent);

            // Finally draw the node on screen
            Draw();
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            BehaviorTreeSelectionManager.hoverObject = this;
        }

        private void InitializeUIElements()
        {
            nodeNameLabel = mainContainer.parent.Q<Label>("NodeTitle");
            SerializedObject serializedNode = new SerializedObject(node);
            
            nodeNameLabel.bindingPath = "nodeName";
            nodeNameLabel.Bind(serializedNode);

            nodeDescriptionLabel = mainContainer.parent.Q<Label>("NodeDescription");
            nodeDescriptionLabel.bindingPath = "description";
            nodeDescriptionLabel.Bind(serializedNode);
            
            decoratorsContainer = mainContainer.parent.Q<VisualElement>("DecoratorsContainer");
            serviceContainer = mainContainer.parent.Q<VisualElement>("ServiceContainer");
            nodeBorder = mainContainer.parent.Q<VisualElement>("selection-border");

            decoratorViews = new List<BT_DecoratorView>();
            serviceViews = new List<BT_ServiceView>();
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

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Node position");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            BehaviorTreeSelectionManager.selectedObject = this;
            ShowSelectionBorder(5f);
            OnNodeSelected.Invoke(this);
        }

        public override void OnUnselected()
        {
            ShowSelectionBorder(0f);
        }

        // Called when the user wants to open the contextual menu while having selected this node view
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            // When the user opens the menu while having selected a node view, show him 
            // all the decorator and service nodes
            var decoratorTypes = TypeCache.GetTypesDerivedFrom<BT_Decorator>();
            foreach (var type in decoratorTypes)
            {
                evt.menu.AppendAction("Decorator/" + type.Name, (a) => behaviorTreeGraph.CreateAttachedNode(type, this));
            }

            var serviceTypes = TypeCache.GetTypesDerivedFrom<BT_Service>();
            foreach(var type in serviceTypes)
            {
                evt.menu.AppendAction("Service/" + type.Name, (a) => behaviorTreeGraph.CreateAttachedNode(type, this));
            }
        }

        public void ShowSelectionBorder(float width)
        {
            nodeBorder.style.color = Color.blue;
            nodeBorder.style.borderRightWidth = width;
            nodeBorder.style.borderLeftWidth = width;
            nodeBorder.style.borderTopWidth = width;
            nodeBorder.style.borderBottomWidth = width;
        }
        
        public void SortChildrenNodes()
        {
            BT_CompositeNode compositeNode = node as BT_CompositeNode;
            if (compositeNode != null)
            {
                compositeNode.childrens.Sort(SortByPosition);
            }
        }

        // Sort behavior tree nodes by horizontal position in the graph
        private int SortByPosition(BT_Node left, BT_Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }
    }
}

