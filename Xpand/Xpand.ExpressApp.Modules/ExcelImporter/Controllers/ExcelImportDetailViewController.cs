using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Validation;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class ExcelImportDetailViewController : ObjectViewController<DetailView,ExcelImport>{
        
        public const string ExcelMappingActionName = "ExcelMapping";
        public const string ImportExcelActionName = "ImportExcel";
        public SimpleAction ExcelMappingAction{ get; }

        public ExcelImportDetailViewController(){
            ExcelMappingAction = new SimpleAction(this,ExcelMappingActionName,"ExcelImport"){Caption = "Map"};
            ExcelMappingAction.Execute+=ExcelMappingActionOnExecute;
            
            ImportAction = new SimpleAction(this,ImportExcelActionName,"ExcelImport"){Caption = "Import"};
            ImportAction.Execute+=ImportActionOnExecute;
            ImportAction.Executing+=ImportActionOnExecuting;
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
            
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
            if (Equals(e.Object, ExcelImport) && e.PropertyName == nameof(BusinessObjects.ExcelImport.FullName) ||
                e.PropertyName == nameof(BusinessObjects.ExcelImport.SheetName) ||
                e.PropertyName == nameof(BusinessObjects.ExcelImport.UseHeaderRows)||
                e.PropertyName == nameof(BusinessObjects.ExcelImport.Type)){

                ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
                TypeChange();
            }
        }

        protected virtual void TypeChange(){
        }

        private void ImportActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            ValidateFile();
        }

        protected ExcelImport ExcelImport => ((ExcelImport) View.CurrentObject);
        protected void ValidateFile(){
            if (ExcelImport.File.Content == null){
                var result = Validator.RuleSet.NewRuleSetValidationMessageResult(ObjectSpace, "Invalid file", "Save",
                    View.CurrentObject, View.ObjectTypeInfo.Type, new List<string>{nameof(ExcelImport.File)});
                throw new ValidationException("", result);
            }
        }

        private void ImportActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var importToTypeInfo = ExcelImport.Type.GetTypeInfo();
            if (ExcelImport.ImportStrategy != ImportStrategy.CreateAlways && importToTypeInfo.DefaultMember == null)
                throw new UserFriendlyException(
                    $"{Application.Model.BOModel.GetClass(importToTypeInfo.Type)} DefaultMember is not set, please use the {nameof(ImportStrategy.CreateAlways)} strategy instead.");
            var index = ExcelImport.Import();
            View.FindItem(
                    $"{nameof(BusinessObjects.ExcelImport.FailedResultList)}.{nameof(FailedResultList.FailedResults)}")
                .Refresh();
            DisplayResultMessage(index);
        }

        private void DisplayResultMessage(int index){
            var failedResults = ExcelImport.FailedResultList.FailedResults;
            string message;
            if (failedResults.Any()){
                
                message = string.Format(CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                    ExcelImporterLocalizationUpdater.ImportFailed), failedResults.GroupBy(r => r.Index).Count(), index);
            }
            else{
                message =
                    string.Format(
                        CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                            ExcelImporterLocalizationUpdater.ImportSucceded), index);
            }
            throw new UserFriendlyException(message);
        }

        public SimpleAction ImportAction{ get;  }

        private void ExcelMappingActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
            
            using (var memoryStream = new MemoryStream(ExcelImport.File.Content)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    using (var dataSet = excelDataReader.GetDataSet(ExcelImport )){
                        foreach (var dataColumn in dataSet.Tables.Cast<DataTable>().First(table => table.TableName==ExcelImport.SheetName).Columns.Cast<DataColumn>()){
                            var excelColumnMap = ObjectSpace.CreateObject<ExcelColumnMap>();
                            ExcelImport.ExcelColumnMaps.Add(excelColumnMap);
                            excelColumnMap.ExcelColumnName = dataColumn.ColumnName;
                            excelColumnMap.PropertyName= ExcelImport.TypePropertyNames.FirstOrDefault(s => s.ToLower()==GetColumnName(excelColumnMap).ToLower());
                            var listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor => editor.MemberInfo.Name==nameof(BusinessObjects.ExcelImport.ExcelColumnMaps));
                            listPropertyEditor.ListView.CollectionSource.Add(excelColumnMap);
                        }
                    }

                    
                    View.FindItem(nameof(BusinessObjects.ExcelImport.ExcelColumnMaps)).Refresh();
                    
                }
            }
        }

        private static string GetColumnName(ExcelColumnMap excelColumnMap){
            return !string.IsNullOrWhiteSpace(excelColumnMap.ExcelImport.ColumnMappingRegexPattern)
                ? Regex.Replace(excelColumnMap.ExcelColumnName,
                    excelColumnMap.ExcelImport.ColumnMappingRegexPattern,
                    excelColumnMap.ExcelImport.ColumnMappingReplacement)
                : excelColumnMap.ExcelColumnName;
        }
    }
}