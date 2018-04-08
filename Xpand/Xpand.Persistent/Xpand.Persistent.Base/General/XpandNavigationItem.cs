using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class XpandNavigationItemAttribute : Attribute, ISupportViewId {
        public XpandNavigationItemAttribute(string path, string viewId=null, int index = -1) 
            : this(path, viewId, null, index) {}

        public XpandNavigationItemAttribute(string path, string viewId, string objectKey, int index = -1) {
            Path = path;
            Index = -1;
            ViewId = viewId;
            Index = index;
            ObjectKey = objectKey;
			
            var paths = Path.Split('/');
            Id = paths[0];
        }
        
        public XpandNavigationItemAttribute(string path, string viewId, string objectKey,  string id, int index = -1) {
            Path = path;
            Index = -1;
            ViewId = viewId;
            Index = index;
            ObjectKey = objectKey;
            Id =  id;
        }
		
        public string Id { get; set; }
		
        public int Index{ get; }

        public string Path{ get; }

        public string ObjectKey { get; set; }

        public string ViewId{ get; }
    }

    public class XpandNavigationItemNodeUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelClasses = node.Application.BOModel.Where(modelClass => modelClass.TypeInfo.FindAttribute<XpandNavigationItemAttribute>() != null);
            foreach (var modelClass in modelClasses) {
                var navigationItemAttributes = modelClass.TypeInfo.FindAttributes<XpandNavigationItemAttribute>();
                foreach (var itemAttribute in navigationItemAttributes) {
                    var paths = itemAttribute.Path.Split('/');
                    AddNodes(((IModelRootNavigationItems)node).Items, paths.ToList(), ViewIds(itemAttribute, modelClass), itemAttribute.ObjectKey, itemAttribute.Index);
                }
            }
        }

        string[] ViewIds(XpandNavigationItemAttribute itemAttribute, IModelClass modelClass) {
            var ns = modelClass.TypeInfo.Type.Namespace;
            var viewId = itemAttribute.ViewId??modelClass.DefaultListView.Id;
            return new[] {$"{ns}.{viewId}", viewId };
        }

        void AddNodes(IModelNavigationItems navigationItems, List<string> strings, string[] viewIds, string objectKey, int index) {
            if (strings.Count == 0) {
                var modelView = navigationItems.Application.Views[viewIds[0]];
                if (modelView == null) {
                    modelView = navigationItems.Application.Views[viewIds[1]];
                    if (modelView == null)
                        throw new NullReferenceException(string.Join("/", viewIds) + " not found in Application.Views");
                }
                ((IModelNavigationItem)navigationItems.Parent).View = modelView;
                return;
            }
            var id = strings[0];
            IModelNavigationItem navigationItem = GetNavigationItem(navigationItems, id, objectKey, strings.Count == 1 ? index : -1);
            strings.RemoveAt(0);
            AddNodes(navigationItem.Items, strings, viewIds, objectKey, index);
        }

        IModelNavigationItem GetNavigationItem(IModelNavigationItems navigationItems, string id, string objectKey, int index) {
            IModelNavigationItem navigationItem;
            if (navigationItems[id] != null)
                navigationItem = navigationItems[id];
            else {
                navigationItem = navigationItems.AddNode<IModelNavigationItem>(id);
                navigationItem.Caption = id;
                navigationItem.ObjectKey = objectKey;
                if (index > -1)
                    navigationItem.Index = index;
            }
            return navigationItem;
        }
    }
}