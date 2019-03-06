using System;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.Utils.Helpers;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    [SecuritySafeCritical]
    public static class ViewExtensions {
        public static bool Fits(this View view,ViewType viewType=ViewType.Any,Nesting nesting=Nesting.Any,Type objectType=null) {
            objectType = objectType ?? typeof(object);
            return FitsCore(view, viewType)&&FitsCore(view,nesting)&&objectType.IsAssignableFrom(view.ObjectTypeInfo.Type);
        }

        private static bool FitsCore(View view, Nesting nesting) {
            return nesting == Nesting.Nested ? !view.IsRoot : nesting != Nesting.Root || view.IsRoot;
        }

        private static bool FitsCore(View view, ViewType viewType){
            if (view == null)
                return false;
            if (viewType == ViewType.ListView)
                return view is ListView;
            if (viewType == ViewType.DetailView)
                return view is DetailView;
            if (viewType == ViewType.DashboardView)
                return view is DashboardView;
            return true;
        }

        public static void Clean(this DetailView detailView,Frame frame) {
            frame.CleanDetailView();
        }

        private static ILayoutManager GetLayoutManager(bool isLayoutSimple,bool delayedViewItemsInitialization){
            var application = ApplicationHelper.Instance.Application;
            var typeInfo = ReflectionHelper
                .FindTypeDescendants(application.TypesInfo.FindTypeInfo(typeof(ILayoutManager)))
                .LastOrDefault();
            if (typeInfo != null){
                if (application.GetPlatform()==Platform.Web) {
                    return (ILayoutManager) typeInfo.Type.CreateInstance(isLayoutSimple,delayedViewItemsInitialization, WebApplicationStyleManager.IsNewStyle);
                }
                return (ILayoutManager) typeInfo.Type.CreateInstance(isLayoutSimple,delayedViewItemsInitialization);
            }

            return null;
        }

        public static void UpdateLayoutManager(this CompositeView compositeView) {
            if (!(compositeView.LayoutManager is ILayoutManager)) {
                var layoutManager = GetLayoutManager(compositeView is ListView,compositeView.DelayedItemsInitialization);
                if (layoutManager != null)
                    compositeView.SetPropertyInfoBackingFieldValue(view => compositeView.LayoutManager, compositeView, layoutManager);
            }
        }
    }
}
