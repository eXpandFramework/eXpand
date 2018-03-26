using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Validation;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class ExcelImportDetailViewController : ObjectViewController<DetailView,ExcelImport>{
        private IModelMember _propertyNameModelMember;
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
            _propertyNameModelMember = Application.Model.BOModel.GetClass(typeof(ExcelColumnMap)).FindMember(nameof(ExcelColumnMap.PropertyName));
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
            if (Equals(e.Object, ExcelImport) && e.PropertyName != nameof(BusinessObjects.ExcelImport.Name) &&
                e.PropertyName != nameof(BusinessObjects.ExcelImport.File) &&
                e.PropertyName != nameof(BusinessObjects.ExcelImport.SheetNames)){

                ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
                _propertyNameModelMember.PredefinedValues = string.Join(";", ExcelImport.TypePropertyNames);
            }
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
            
            ExcelImport.FailedResultList.FailedResults.Clear();
            using (var memoryStream = new MemoryStream(ExcelImport.File.Content)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    int index;
                    using (var dataSet = GetDataSet(excelDataReader,ExcelImport)){
                        index = 0;
                        var importToTypeInfo = ExcelImport.Type.GetTypeInfo();
                        var columnMembers = ExcelImport.ExcelColumnMaps.Select(map => (column: map.ExcelColumnName,
                            memberInfo: importToTypeInfo.Members.First(info =>!string.IsNullOrEmpty(info.DisplayName)
                                    ? info.DisplayName == map.PropertyName: info.Name == map.PropertyName))).ToArray();
                        foreach (var dataRow in dataSet.Tables.Cast<DataTable>().First().Rows.Cast<DataRow>()){
                            index++;
                            var importToObject = ObjectSpace.CreateObject(importToTypeInfo.Type);
                            Import(ExcelImport, dataRow, importToObject, index,columnMembers);
                        }
                    }
                    View.FindItem($"{nameof(BusinessObjects.ExcelImport.FailedResultList)}.{nameof(FailedResultList.FailedResults)}").Refresh();
                    DisplayResultMessage(index);
                }

            }
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

        protected static DataSet GetDataSet(IExcelDataReader excelDataReader, ExcelImport excelImport){
            var excelDataSetConfiguration = new ExcelDataSetConfiguration{
                ConfigureDataTable = reader => new ExcelDataTableConfiguration(){
                    UseHeaderRow = excelImport.UseHeaderRows,
                    ReadHeaderRow= dataReader => {
                        for (int i = 1; i < excelImport.HeaderRows; i++){
                            dataReader.Read();
                        }
                    } 
                },
                UseColumnDataType = false
            };
            return excelDataReader.AsDataSet(excelDataSetConfiguration);
        }

        private void Import(ExcelImport excelImport, DataRow dataRow,
            object importToObject, int index, IEnumerable<(string column, IMemberInfo memberInfo)> columnMembers){

            var results=new List<FailedResult>();
            foreach (var columnMember in columnMembers){
                var columnValue = dataRow[columnMember.column];
                if (!Imported(columnValue, columnMember.memberInfo,importToObject)){
                    var importResult = new FailedResult{
                        ExcelColumnValue = columnValue?.ToString(),
                        ExcelColumnName = columnMember.column
                    };
                    results.Add(importResult);   
                }                
            }

            foreach (var failedResult in results){
                var importResultImportedObject = importToObject.ToString();
                failedResult.ImportedObject = importResultImportedObject;
                failedResult.Index = index;
                excelImport.FailedResultList.FailedResults.Add(failedResult);
            }
        }

        private bool Imported(object columnValue, IMemberInfo memberInfo, object importToObject){
            var memberTypeInfo = memberInfo.MemberTypeInfo;
            object result;
            if (memberTypeInfo.IsPersistent){
                var type = memberTypeInfo.Type;
                if (columnValue.TryToChange(memberTypeInfo.DefaultMember.MemberType, out result)){
                    try{
                        var referenceObject =ObjectSpace.FindObject(type,CriteriaOperator.Parse($"{memberTypeInfo.DefaultMember.Name}=?", result), true) ??
                                             ObjectSpace.CreateObject(type);
                        memberTypeInfo.DefaultMember.SetValue(referenceObject,result);
                        memberInfo.SetValue(importToObject,referenceObject);
                    }
                    catch (Exception e){
                        Tracing.Tracer.LogError(e);
                        return false;
                    }
                    return true;
                }

                return false;
            }

            var tryToChange = columnValue.TryToChange(memberInfo.MemberType, out result);
            memberInfo.SetValue(importToObject,result);
            return tryToChange;
        }

        public SimpleAction ImportAction{ get;  }

        private void ExcelMappingActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
            
            using (var memoryStream = new MemoryStream(ExcelImport.File.Content)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    using (var dataSet = GetDataSet(excelDataReader,ExcelImport )){
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