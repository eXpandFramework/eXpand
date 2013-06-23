using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Model;

namespace Xpand.ExpressApp.Web.ViewStrategies {
    public class XpandShowViewStrategy : ShowViewStrategy {
        public XpandShowViewStrategy(XafApplication application)
            : base(application) {
        }
        protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            if (((IModelListViewOpenViewWhenNested)showViewSource.SourceFrame.View.Model).OpenViewWhenNestedStrategy==OpenViewWhenNestedStrategy.InMainWindow)
                Application.MainWindow.SetView(parameters.CreatedView, showViewSource.SourceFrame);
            else {
                base.ShowViewFromNestedView(parameters, showViewSource);
            }
        }

    }
}
