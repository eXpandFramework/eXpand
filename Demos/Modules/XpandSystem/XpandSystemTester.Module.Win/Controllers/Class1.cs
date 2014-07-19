using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;

namespace XpandSystemTester.Module.Win.Controllers{
    public class GridColumnFilterPopupMaxRecordsCountController : ViewController<ListView>{
        public GridColumnFilterPopupMaxRecordsCountController(){
            TargetViewType = ViewType.ListView;
            TypeOfView = typeof (ListView);
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();

//            if (View.Editor.GetType() == typeof (XpandGridListEditor)){
                var listEditor = View.Editor as GridListEditor;

                var columnViewOptionsFilter = listEditor.GridView.OptionsFilter;
                columnViewOptionsFilter.ColumnFilterPopupMaxRecordsCount = 0;
                columnViewOptionsFilter.ColumnFilterPopupRowCount = 0;
//            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
        }
    }
}