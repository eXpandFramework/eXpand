using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using System.Linq;
using eXpand.ExpressApp.Attributes;

namespace eXpand.ExpressApp.NodeUpdaters {
    public class XpandNavigationItemNodeUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator>
    {
        public override void UpdateNode(ModelNode node) {
            var modelClasses = node.Application.BOModel.Where(modelClass => modelClass.TypeInfo.FindAttribute<XpandNavigationItemAttribute>()!=null);
            foreach (var modelClass in modelClasses) {
                var navigationItemAttributes = modelClass.TypeInfo.FindAttributes<XpandNavigationItemAttribute>();
                foreach (var itemAttribute in navigationItemAttributes) {
                    var paths = itemAttribute.Path.Split('/');
                    AddNodes(((IModelRootNavigationItems)node).Items, paths.ToList(), itemAttribute.ViewId,itemAttribute.ObjectKey);   
                }
            }
        }

        void AddNodes(IModelNavigationItems navigationItems, List<string> strings, string viewId, string objectKey) {
            if (strings.Count == 0) {
                var modelView = navigationItems.Application.Views.Where(view => view.Id == viewId).FirstOrDefault();
                if (modelView== null)
                    throw new NullReferenceException(viewId);
                ((IModelNavigationItem) navigationItems.Parent).View =modelView;
                return;
            }
            var id = strings[0];
            IModelNavigationItem navigationItem = GetNavigationItem(navigationItems, id,objectKey);
            strings.RemoveAt(0);
            AddNodes(navigationItem.Items, strings,viewId,objectKey);
        }

        IModelNavigationItem GetNavigationItem(IModelNavigationItems navigationItems, string id, string objectKey) {
            IModelNavigationItem navigationItem;
            if (navigationItems.GetNode<IModelNavigationItem>(id) != null)
                navigationItem = navigationItems.GetNode<IModelNavigationItem>(id);
            else {
                navigationItem = navigationItems.AddNode<IModelNavigationItem>(id);
                navigationItem.Caption = id;
                navigationItem.ObjectKey = objectKey;
            }
            return navigationItem;
        }

    }
}