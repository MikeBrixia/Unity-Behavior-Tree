using System;
using System.Collections.Generic;
using BT;
using BT.Editor;
using BT.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.BehaviorTree.BT_Elements
{
    public class BT_ActionView : BT_ParentNodeView
    {
        ///<summary>
        /// decorator views contained inside this node view
        ///</summary>
        private readonly List<BT_DecoratorView> decoratorViews;

        ///<summary>
        /// service views contained inside this node view
        ///</summary>
        private readonly List<BT_ServiceView> serviceViews;
        
        /// <summary>
        /// The filepath of the action element UXML file.
        /// </summary>
        private const string ACTION_PATH = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/ParentNodeViews/NodeView.uxml";
        
        ///<summary>
        /// The displayed node description
        ///</summary>
        private Label nodeDescriptionLabel;
        
        /// <summary>
        /// The border of this node.
        /// </summary>
        private VisualElement nodeBorder;
        
        ///<summary>
        /// The displayed node name
        ///</summary>
        private Label nodeNameLabel;
        
        /// <summary>
        /// The displayed node type name.
        /// </summary>
        private Label nodeTypeNameLabel;
        
        ///<summary>
        /// Container for decorator nodes
        ///</summary>
        private VisualElement decoratorsContainer;

        ///<summary>
        /// Container service containers
        ///</summary>
        private VisualElement serviceContainer;
        
        public BT_ActionView(BT_ParentNode node, BehaviorTreeGraphView graph) : base(node, graph, ACTION_PATH)
        {
            decoratorViews = new List<BT_DecoratorView>();
            serviceViews = new List<BT_ServiceView>();
        }

        protected override void InitializeUIElements()
        {
            nodeNameLabel = mainContainer.parent.Q<Label>("NodeTitle");
            nodeTypeNameLabel = mainContainer.parent.Q<Label>("NodeTypeName");
            SerializedObject serializedNode = new SerializedObject(node);

            // Bind node name value to label
            nodeNameLabel.bindingPath = "nodeName";
            nodeNameLabel.Bind(serializedNode);

            // Bind node type name value to label
            nodeTypeNameLabel.bindingPath = "nodeTypeName";
            nodeTypeNameLabel.Bind(serializedNode);

            // Bind description value to description label.
            nodeDescriptionLabel = mainContainer.parent.Q<Label>("NodeDescription");
            nodeDescriptionLabel.bindingPath = "description";
            nodeDescriptionLabel.Bind(serializedNode);

            decoratorsContainer = mainContainer.parent.Q<VisualElement>("DecoratorsContainer");
            serviceContainer = mainContainer.parent.Q<VisualElement>("ServiceContainer");
            nodeBorder = mainContainer.parent.Q<VisualElement>("selection-border");
        }

        public override void OnSelected()
        {
            base.OnSelected();
            ShowSelectionBorder(nodeBorder, 5f);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            ShowSelectionBorder(nodeBorder, 0f);
        }

        public override List<T> GetChildViews<T>()
        {
            List<T> resultList = null;
            
            if (typeof(T) == typeof(BT_DecoratorView))
                resultList = decoratorViews as List<T>;
            else if (typeof(T) == typeof(BT_ServiceView))
                resultList = serviceViews as List<T>;

            return resultList;
        }

        public override void AddChildView<T>(T childView)
        {
            Type nodeType = typeof(T);
            if (nodeType == typeof(BT_DecoratorView))
            {
                decoratorViews.Add(childView as BT_DecoratorView);
                decoratorsContainer.Add(childView.contentContainer);
            }
            else if (nodeType == typeof(BT_ServiceView))
            {
                serviceViews.Add(childView as BT_ServiceView);
                serviceContainer.Add(childView.contentContainer);
            }
        }

        public override void CreateChildViews()
        {
            if (node != null && node is not BT_RootNode)
            {
                BT_ChildNodeView view;
                // Create decorators child views.
                List<BT_Decorator> decorators = node.GetChildNodes<BT_Decorator>();
                foreach (BT_Decorator decorator in decorators)
                {
                    view = NodeFactory.CreateChildNodeView(this, decorator, graph);
                    AddChildView<BT_DecoratorView>((BT_DecoratorView) view);
                }
                
                // Create services child views.
                List<BT_Service> services = node.GetChildNodes<BT_Service>();
                foreach (BT_Service service in services)
                {
                    view = NodeFactory.CreateChildNodeView(this, service, graph);
                    AddChildView<BT_ServiceView>((BT_ServiceView)view);
                }
            }
        }
    }
}