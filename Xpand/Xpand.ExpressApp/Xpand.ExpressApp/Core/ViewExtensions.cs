using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Core {
    public static class ViewExtensions {
        public static ILayoutManager LayoutManager {
            get {
                var typeInfo =ReflectionHelper.FindTypeDescendants(ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo(typeof (ILayoutManager))).FirstOrDefault();
                return typeInfo != null ? (ILayoutManager) ReflectionHelper.CreateObject(typeInfo.Type) : null;
            }
        }

        public static void UpdateLayoutManager(this CompositeView compositeView) {
            if (!(compositeView.LayoutManager is ILayoutManager)) {
                var layoutManager = LayoutManager;
                if (layoutManager != null)
                    compositeView.SetPropertyInfoBackingFieldValue(view => compositeView.LayoutManager, compositeView, layoutManager);
            }
        }
    }
}
