using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Core {
    public static class ViewExtensions {
        public static ILayoutManager LayoutManager {
            get {
                var valueManager = ValueManager.GetValueManager<ILayoutManager>("ILayoutManager");
                if (valueManager.Value == null) {
                    var typeInfo = ReflectionHelper.FindTypeDescendants(ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo(typeof(ILayoutManager))).FirstOrDefault();
                    if (typeInfo != null)
                        valueManager.Value = (ILayoutManager) ReflectionHelper.CreateObject(typeInfo.Type);
                }
                return valueManager.Value;
            }
        }

        public static void UpdateLayoutManager(this CompositeView compositeView) {
            if (!(compositeView.LayoutManager is ILayoutManager)&&LayoutManager!=null) {
                compositeView.SetPropertyInfoBackingFieldValue(view => compositeView.LayoutManager, compositeView, LayoutManager);
            }
        }
    }
}
