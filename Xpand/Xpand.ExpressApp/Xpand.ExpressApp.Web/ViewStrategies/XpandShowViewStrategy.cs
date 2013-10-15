using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Model;

namespace Xpand.ExpressApp.Web.ViewStrategies {
    public class XpandShowViewStrategy : ShowViewStrategy {
        public XpandShowViewStrategy(XafApplication application)
            : base(application) {
        }
        protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            var model =showViewSource.SourceFrame.View.Model as IModelListViewOpenViewWhenNested;
            if (model != null) {
                if (model.OpenViewWhenNestedStrategy == OpenViewWhenNestedStrategy.InMainWindow)
                    Application.MainWindow.SetView(parameters.CreatedView, showViewSource.SourceFrame);
                else
                    base.ShowViewFromNestedView(parameters, showViewSource);
            }
        }

        protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            var model =showViewSource.SourceView.Model as IModelListViewOpenViewWhenNested;
            if (model != null && model.OpenDetailViewAsPopup) {
                base.ShowViewInModalWindow(parameters, showViewSource);
            }
            else {
                base.ShowViewFromCommonView(parameters, showViewSource);
            }
        }

    }
}
