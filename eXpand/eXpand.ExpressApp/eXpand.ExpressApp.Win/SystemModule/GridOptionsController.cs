using DevExpress.Utils.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public class GridOptionsController : ExpressApp.SystemModule.GridOptionsController<GridView, BaseOptions>
    {
        protected override object GetControl()
        {
            var control = (GridControl)base.GetControl();
            return control.MainView;
        }
    }
}