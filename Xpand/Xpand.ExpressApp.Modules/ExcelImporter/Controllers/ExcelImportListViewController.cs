//using System;
//using System.Linq;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.Persistent.Base;
//using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
//
//namespace Xpand.ExpressApp.ExcelImporter.Controllers{
//    public class ExcelImportListViewController : ViewController<ListView>{
//        public SingleChoiceAction ExcelImport{ get; }
//
//        public ExcelImportListViewController(){
//            ExcelImport =new SingleChoiceAction(this, "ExcelImport", PredefinedCategory.Export){
//                    ItemType = SingleChoiceActionItemType.ItemIsOperation,
//                    DefaultItemMode = DefaultItemMode.LastExecutedItem
//                };
//
//            ExcelImport.Execute+=ExcelImportOnExecute;
//        }
//
//        private void ExcelImportOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
//            var showViewParameters = e.ShowViewParameters;
//            var objectSpace = Application.CreateObjectSpace();
//            var data = e.SelectedChoiceActionItem.Data;
//            var excelImport = data != null 
//                && objectSpace.GetObjectsQuery<ExcelImport>().FirstOrDefault(configuration => configuration.Oid == (Guid) data) != null
//                ? (objectSpace.GetObjectsQuery<ExcelImport>().FirstOrDefault(configuration => configuration.Oid == (Guid) data) ??
//                   objectSpace.CreateObject<ExcelImport>()): objectSpace.CreateObject<ExcelImport>();
//            excelImport.Type = View.ObjectTypeInfo.Type;
//            var detailView = Application.CreateDetailView(objectSpace, excelImport);
//            detailView.ViewEditMode=ViewEditMode.Edit;
//            showViewParameters.CreatedView = detailView;
//            var dialogController = new DialogController();
//            dialogController.AcceptAction.Executed += (o, args) => {
//                ObjectSpace.Refresh();
//                UpdateChoiceItems();
//            };
//            showViewParameters.Controllers.Add(dialogController);
//            showViewParameters.TargetWindow=TargetWindow.NewModalWindow;
//        }
//
//        protected override void OnActivated(){
//            base.OnActivated();
//            UpdateChoiceItems();
//        }
//
//        private void UpdateChoiceItems(){
//            ExcelImport.Items.Clear();
//            ExcelImport.Items.Add(new ChoiceActionItem("New", null));
//            ExcelImport.Items.AddRange(ObjectSpace
//                .GetObjectsQuery<ExcelImport>()
//                .ToArray()
//                .Where(configuration => configuration.Type == View.ObjectTypeInfo.Type)
//                .Select(configuration => new ChoiceActionItem(configuration.Name, configuration.Oid)).ToArray());
//        }
//    }
//}