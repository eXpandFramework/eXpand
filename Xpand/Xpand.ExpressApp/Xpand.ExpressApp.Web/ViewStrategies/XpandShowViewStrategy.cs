using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.ViewStrategies {
    public class XpandShowViewStrategy : ShowViewStrategy {
        public XpandShowViewStrategy(XafApplication application)
            : base(application) {
        }
        protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            var model =showViewSource.SourceFrame.View.Model as IModelListViewOpenViewWhenNested;
            if (model != null) {
                if (model.OpenViewWhenNestedStrategy == OpenViewWhenNestedStrategy.InMainWindow) {
                    var view = Application.CreateView(parameters.CreatedView.Model);
                    var currentObject = view.ObjectSpace.GetObject(parameters.CreatedView.CurrentObject);
                    view.CurrentObject=currentObject;
                    Application.MainWindow.SetView(view,showViewSource.SourceFrame);
                }
                else
                    base.ShowViewFromNestedView(parameters, showViewSource);
            }
            else
                base.ShowViewFromNestedView(parameters, showViewSource);
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
