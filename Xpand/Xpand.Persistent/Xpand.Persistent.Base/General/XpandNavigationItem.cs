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
            Caption = Id;
        }
        
        public XpandNavigationItemAttribute(string path, string viewId, string objectKey, string id, int index = -1) {
            Path = path;
            Index = -1;
            ViewId = viewId;
            Index = index;
            ObjectKey = objectKey;
            Id =  id;
        }
        
        public XpandNavigationItemAttribute(string path, string viewId, string objectKey, string id, string caption, int index = -1) {
            Path = path;
            Index = -1;
            ViewId = viewId;
            Index = index;
            ObjectKey = objectKey;
            Id = id;
            Caption = caption;
        }

        public string Id { get; set; }

        public string Caption { get; set; }

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
                    var paths = itemAttribute.Path.Split('/').ToList();
                    if (paths.Count > 0) paths.RemoveAt(paths.Count -1);
                    var parentItems = GetParentNavigationItems(((IModelRootNavigationItems)node).Items, paths);
                    AddNavigationItem(parentItems, ViewIds(itemAttribute, modelClass), itemAttribute);
                }
            }
        }

        private IModelNavigationItems GetParentNavigationItems(IModelNavigationItems rootItems, List<string> paths) {
            var parentNavigationItems = rootItems;
            foreach (var path in paths) {
                if (parentNavigationItems[path] != null) {
                    parentNavigationItems = parentNavigationItems[path].Items;
                }
                else {
                    var navigationItem = parentNavigationItems.AddNode<IModelNavigationItem>(path);
                    navigationItem.Index = parentNavigationItems.Count;
                    parentNavigationItems = navigationItem.Items;
                }
            }
            return parentNavigationItems;
        }

        string[] ViewIds(XpandNavigationItemAttribute itemAttribute, IModelClass modelClass) {
            var ns = modelClass.TypeInfo.Type.Namespace;
            var viewId = itemAttribute.ViewId??modelClass.DefaultListView.Id;
            return new[] {$"{ns}.{viewId}", viewId };
        }

        void AddNavigationItem(IModelNavigationItems navigationItems, string[] viewIds, XpandNavigationItemAttribute itemAttribute) {
            var modelView = navigationItems.Application.Views[viewIds[0]];
            if (modelView == null) {
                modelView = navigationItems.Application.Views[viewIds[1]];
                if (modelView == null)
                    throw new NullReferenceException(string.Join("/", viewIds) + " not found in Application.Views");
            }
            
            var navigationItem = navigationItems.AddNode<IModelNavigationItem>(itemAttribute.Id);
            navigationItem.Caption = itemAttribute.Caption;
            navigationItem.ObjectKey = itemAttribute.ObjectKey;
            navigationItem.View = modelView;
            if (itemAttribute.Index > -1)
                navigationItem.Index = itemAttribute.Index;

        }
    }
}
