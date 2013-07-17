using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using System.Linq;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.NodeUpdaters {
    public class XpandNavigationItemNodeUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelClasses = node.Application.BOModel.Where(modelClass => modelClass.TypeInfo.FindAttribute<XpandNavigationItemAttribute>() != null);
            foreach (var modelClass in modelClasses) {
                var navigationItemAttributes = modelClass.TypeInfo.FindAttributes<XpandNavigationItemAttribute>();
                foreach (var itemAttribute in navigationItemAttributes) {
                    var paths = itemAttribute.Path.Split('/');
                    AddNodes(((IModelRootNavigationItems)node).Items, paths.ToList(), ViewIds(itemAttribute, modelClass.TypeInfo.Type.Namespace), itemAttribute.ObjectKey, itemAttribute.Index);
                }
            }
        }

        string[] ViewIds(XpandNavigationItemAttribute itemAttribute, string ns) {
            return new[] { string.Format("{0}.{1}", ns, itemAttribute.ViewId), itemAttribute.ViewId };
        }

        void AddNodes(IModelNavigationItems navigationItems, List<string> strings, string[] viewIds, string objectKey, int index) {
            if (strings.Count == 0) {
                var modelView = navigationItems.Application.Views[viewIds[0]];
                if (modelView == null) {
                    modelView = navigationItems.Application.Views[viewIds[1]];
                    if (modelView==null)
                        throw new NullReferenceException(viewIds + " not found in Application.Views");
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