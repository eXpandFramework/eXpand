using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Model;

namespace Xpand.ExpressApp.Web.ViewStrategies {
    public class XpandShowViewStrategy : ShowViewStrategy {
        public XpandShowViewStrategy(XafApplication application)
            : base(application) {
        }
        protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            IModelListViewOpenViewWhenNested model =
                showViewSource.SourceFrame.View.Model as IModelListViewOpenViewWhenNested;

            if (model != null) {
                if (model.OpenViewWhenNestedStrategy == OpenViewWhenNestedStrategy.InMainWindow)
                    Application.MainWindow.SetView(parameters.CreatedView, showViewSource.SourceFrame);
                else if (model.OpenDetailViewAsPopup) {
                    base.ShowViewInModalWindow(parameters, showViewSource);
                }
                else {
                    base.ShowViewFromNestedView(parameters, showViewSource);
                }
            }
        }

    }
}
