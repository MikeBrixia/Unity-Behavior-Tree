
using System;
using System.Collections.Generic;
using BT;
using BT.Editor;
using BT.Runtime;
using UnityEngine;

namespace Editor.BehaviorTree.BT_Elements
{
    public class BT_ActionView : BT_ParentNodeView, IParentView
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
        private const string ACTION_PATH = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/NodeView.uxml";
        
        public BT_ActionView(BT_ParentNode node, BehaviorTreeGraphView graph) : base(node, graph, ACTION_PATH)
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
            Type nodeType = typeof(T);
            if (nodeType == typeof(BT_DecoratorView))
                decoratorViews.Add(childView as BT_DecoratorView);
            else if (nodeType == typeof(BT_ServiceView))
                serviceViews.Add(childView as BT_ServiceView);
        }

        public void CreateChildViews()
        {
            if (node != null && node is not BT_RootNode)
            {
                BT_ChildNodeView view;
                // Create decorators child views.
                List<BT_Decorator> decorators = node.GetChildNodes<BT_Decorator>();
                foreach (BT_Decorator decorator in decorators)
                {
                    view = NodeFactory.CreateChildNodeView(this, decorator, graph);
                    decoratorViews.Add((BT_DecoratorView) view);
                }
                
                // Create services child views.
                List<BT_Service> services = node.GetChildNodes<BT_Service>();
                foreach (BT_Service service in services)
                {
                    view = NodeFactory.CreateChildNodeView(this, service, graph);
                    serviceViews.Add((BT_ServiceView) view);
                }
            }
        }
    }
}