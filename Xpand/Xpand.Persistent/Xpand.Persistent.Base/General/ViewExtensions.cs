using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.Utils.Helpers;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    [SecuritySafeCritical]
    public static class ViewExtensions {
        public static void Clean(this DetailView detailView,Frame frame) {
            frame.CleanDetailView();
        }

        public static ILayoutManager LayoutManager {
            get {
                var typeInfo =ReflectionHelper.FindTypeDescendants(Xpand.Persistent.Base.General.ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo(typeof (Xpand.Persistent.Base.General.ILayoutManager))).LastOrDefault();
                return (ILayoutManager) typeInfo?.Type.CreateInstance();
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
