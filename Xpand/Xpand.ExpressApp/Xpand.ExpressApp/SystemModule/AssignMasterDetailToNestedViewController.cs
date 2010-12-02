using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class AssignMasterDetailToNestedViewController : ViewController<DetailView> {
        protected AssignMasterDetailToNestedViewController() {
            TargetViewType = ViewType.DetailView;
            TargetViewNesting = Nesting.Root;
        }
        protected override void OnActivated() {
            base.OnActivated();
            foreach (ListPropertyEditor listPropertyEditor in ((View).GetItems<ListPropertyEditor>())) {
                foreach (Controller c in listPropertyEditor.Frame.Controllers) {
                    if (c is IMasterDetailViewInfo) {
                        ((IMasterDetailViewInfo)c).AssignMasterDetailView(View);
                    }
                }
            }
        }
    }

    public interface IMasterDetailViewInfo {
        void AssignMasterDetailView(DetailView detailView);
    }
}
