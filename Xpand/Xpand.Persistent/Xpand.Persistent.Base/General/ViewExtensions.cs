using System;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
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

        public static ILayoutManager LayoutManager {
            get {
                var typeInfo =ReflectionHelper.FindTypeDescendants(ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo(typeof (ILayoutManager))).LastOrDefault();
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
