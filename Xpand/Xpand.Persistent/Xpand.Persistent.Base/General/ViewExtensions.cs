using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.Utils.Helpers;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    [SecuritySafeCritical]
    public static class ViewExtensions {
        public static ILayoutManager LayoutManager {
            get {
                var typeInfo =ReflectionHelper.FindTypeDescendants(ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo(typeof (ILayoutManager))).FirstOrDefault();
                return typeInfo != null ? (ILayoutManager) typeInfo.Type.CreateInstance() : null;
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
