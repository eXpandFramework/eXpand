using DevExpress.ExpressApp;
using eXpand.ExpressApp.Attributes;

namespace eXpand.ExpressApp.SystemModule {
    public class AutoCreatableObjectController : ViewController<DetailView> {
        void view_CustomizeViewShortcut(object sender, CustomizeViewShortcutArgs e) {
            e.ViewShortcut.ViewId = Application.GetListViewId(View.ObjectTypeInfo.Type);
            e.ViewShortcut.ObjectKey = string.Empty;
            e.ViewShortcut.Remove("mode");
        }

        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            Active.SetItemValue("AutoCreatableObject", false);
            if (view != null) {
                var attribute = view.ObjectTypeInfo.FindAttribute<AutoCreatableObjectAttribute>(true);
                if (attribute != null) {
                    Active.SetItemValue("AutoCreatableObject", true);
                    view.CustomizeViewShortcut += view_CustomizeViewShortcut;
                }
            }
        }

        protected override void OnDeactivating() {
            if (View != null) {
                View.CustomizeViewShortcut -= view_CustomizeViewShortcut;
            }
            base.OnDeactivating();
        }
    }
}