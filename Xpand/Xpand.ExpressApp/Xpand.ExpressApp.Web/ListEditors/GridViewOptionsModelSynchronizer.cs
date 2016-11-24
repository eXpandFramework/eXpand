using DevExpress.ExpressApp.Model;
using DevExpress.Web;
using Xpand.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.ListEditors
{
    public class GridViewOptionsModelSynchronizer : ExpressApp.ListEditors.GridViewOptionsModelSynchronizer
    {
        public GridViewOptionsModelSynchronizer(object control, IModelListView model) : base(control, model) {
        }
        protected override void ApplyModelCore()
        {
            base.ApplyModelCore();
            var gridView = ((ASPxGridView) Control);
            bool? enableCallBacks = ((IModelListViewMainViewOptions)Model).GridViewOptions.EnableCallBacks;
            if (enableCallBacks.HasValue)
                gridView.EnableCallBacks = enableCallBacks.Value;
            if (gridView.Settings.ShowStatusBar!=GridViewStatusBarMode.Visible)
                gridView.Templates.StatusBar = null;
        }
    }
}
