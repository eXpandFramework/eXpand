using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.Controllers;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class MapDialogController : ViewController {
        private readonly ExcelImport _excelImport;

        public MapDialogController(ExcelImport excelImport) {
            _excelImport = excelImport;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var listView = Frame.GetController<MasterDetailController>().ListView;
            listView.CollectionSource.Criteria[GetType().Name] =
                listView.ObjectSpace.GetCriteriaOperator<ExcelColumnMap>(map =>
                    map.ExcelImport.Oid == _excelImport.Oid);
        }
    }
}