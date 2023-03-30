
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
        public List<BT_DecoratorView> decoratorViews { get; private set; }

        ///<summary>
        /// service views contained inside this node view
        ///</summary>
        public List<BT_ServiceView> serviceViews { get; private set; }
        
        public BT_ActionView(BT_Node node, BehaviorTreeGraphView graph) : base(node, graph)
        {
            stylePath = "Packages/com.ai.behavior-tree/Editor/BehaviorTree/BT Elements/NodeView.uxml";
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
            if (node is IParentNode parentNode)
            {
                // Create decorators child views.
                IList<BT_Decorator> decorators = parentNode.GetChildNodes<BT_Decorator>();
                foreach (BT_Decorator decorator in decorators)
                {
                    BT_DecoratorView decoratorView = new BT_DecoratorView(behaviorTreeGraph, this, decorator);
                    decoratorViews.Add(decoratorView);
                }
                
                // Create services child views.
                IList<BT_Service> services = parentNode.GetChildNodes<BT_Service>();
                foreach (BT_Service service in services)
                {
                    BT_ServiceView serviceView = new BT_ServiceView(behaviorTreeGraph, this, service);
                    serviceViews.Add(serviceView);
                }
            }
        }
    }
}