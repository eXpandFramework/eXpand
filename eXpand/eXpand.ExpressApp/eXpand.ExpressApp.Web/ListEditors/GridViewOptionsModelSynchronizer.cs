using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxGridView;

namespace eXpand.ExpressApp.Web.ListEditors
{
    public class GridViewOptionsModelSynchronizer : ExpressApp.ListEditors.GridViewOptionsModelSynchronizer
    {
        public GridViewOptionsModelSynchronizer(object control, IModelListView model) : base(control, model) {
        }
        protected override void ApplyModelCore()
        {
            base.ApplyModelCore();
            var gridView = ((ASPxGridView) Control);
            if (gridView.Settings.ShowStatusBar==GridViewStatusBarMode.Visible)
                gridView.Templates.StatusBar = null;
        }
    }
}
