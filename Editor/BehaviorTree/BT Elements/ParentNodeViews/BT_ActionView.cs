﻿
using System.Collections.Generic;
using BT;
using BT.Editor;
using BT.Runtime;

namespace Editor.BehaviorTree.BT_Elements
{
    public class BT_ActionView : BT_NodeView, IParentView
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
        private const string actionUiFilepath = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/NodeView.uxml";
        
        public BT_ActionView(BT_Node node, BehaviorTreeGraphView graph) : base(node, graph, actionUiFilepath)
        {
            decoratorViews = new List<BT_DecoratorView>();
            serviceViews = new List<BT_ServiceView>();
        }

        public IList<T> GetChildViews<T>() where T : BT_ChildNodeView
        {
            IList<T> resultList = null;
            
            if (typeof(T) == typeof(BT_DecoratorView))
                resultList = decoratorViews as IList<T>;
            else if (typeof(T) == typeof(BT_ServiceView))
                resultList = serviceViews as IList<T>;

            return resultList;
        }

        public void AddChildView<T>(T childView) where T : BT_ChildNodeView, IChildView
        {
            if (typeof(T) == typeof(BT_DecoratorView))
                decoratorViews.Add(childView as BT_DecoratorView);
            else if (typeof(T) == typeof(BT_ServiceView))
                serviceViews.Add(childView as BT_ServiceView);
        }

        public void CreateChildViews()
        {
            if (node is BT_ParentNode parentNode)
            {
                // Create decorators child views.
                IList<BT_Decorator> decorators = parentNode.GetChildNodes<BT_Decorator>();
                foreach (BT_Decorator decorator in decorators)
                {
                    NodeFactory.CreateChildNodeView(parentNode, decorator, behaviorTreeGraph);
                }
                
                // Create services child views.
                IList<BT_Service> services = parentNode.GetChildNodes<BT_Service>();
                foreach (BT_Service service in services)
                {
                    NodeFactory.CreateChildNodeView(parentNode, service, behaviorTreeGraph);
                }
            }
        }
    }
}