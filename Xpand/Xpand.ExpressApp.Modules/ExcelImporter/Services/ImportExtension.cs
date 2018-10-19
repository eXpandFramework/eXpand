using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.Metadata.Helpers;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;
using Xpand.Utils.Linq;
using Type = System.Type;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    public static class ImportExtension{

        public static IMemberInfo FindMember(this ExcelImport excelImport,string member) {
            return excelImport?.Type.GetTypeInfo().Members.WhereMapable().FirstOrDefault(info =>
                CaptionHelper.GetMemberCaption(info.Owner, info.Name).Equals(member, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IMemberInfo> WhereMapable(this IEnumerable<IMemberInfo> memberInfos) {
            return memberInfos.Where(info => info.IsMapable());
        }

        private static bool IsMapable(this IMemberInfo info){
            var isMappable = info.IsPersistent && !info.IsService && info.Name != GCRecordField.StaticName;
            if (isMappable){
                var browsableAttribute = info.FindAttribute<BrowsableAttribute>();
                var isBrowsable = browsableAttribute == null || browsableAttribute.Browsable;
                var visibleInLookupListViewAttribute = info.FindAttribute<VisibleInLookupListViewAttribute>();
                var visibleInLookup = visibleInLookupListViewAttribute == null ||(bool) visibleInLookupListViewAttribute.Value;
                var visibleInListViewAttribute = info.FindAttribute<VisibleInListViewAttribute>();
                var visibleInListView= visibleInListViewAttribute == null ||(bool) visibleInListViewAttribute.Value;
                var visibleInExcelMapAttribute = info.FindAttribute<VisibleInExcelMapAttribute>();
                var visibleInExcelMap = visibleInExcelMapAttribute == null ||visibleInExcelMapAttribute.Visible;
                return (isBrowsable && visibleInLookup && visibleInListView) || visibleInExcelMap;
            }
            return false;
        }

        private static object GetImportToObject(ITypeInfo importToTypeInfo,
            ImportParameter[] columnNames, DataRow dataRow, ExcelImport excelImport){
            var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
            if (excelImport.ImportStrategy==ImportStrategy.CreateAlways)
                return objectSpace.CreateObject(importToTypeInfo.Type);
            var valueTuple = columnNames.First(tuple => tuple.MemberInfo==importToTypeInfo.GetKeyMember());
            var o = dataRow[valueTuple.Map.ExcelColumnName];
            var importToObject = objectSpace.FindObject(importToTypeInfo.Type,CriteriaOperator.Parse($"{valueTuple.MemberInfo.Name}=?", o));
            if (excelImport.ImportStrategy == ImportStrategy.UpdateOnly) {
                return importToObject;
            }
            if (excelImport.ImportStrategy==ImportStrategy.UpdateOrCreate){
                return importToObject ?? objectSpace.CreateObject(importToTypeInfo.Type);
            }
            return importToObject != null ? null : objectSpace.CreateObject(importToTypeInfo.Type);
        }

        struct ImportParameter {
            public IMemberInfo MemberInfo { get; set; }
            public ExcelColumnMap Map { get; set; }
        }
        private static void Import(ExcelImport excelImport, DataRow dataRow,object importToObject, int index,
            ImportParameter[] importParameterses) {

            var results=new List<FailedResult>();
            foreach (var importParameter in importParameterses){
                var columnValue = dataRow[importParameter.Map.ExcelColumnName];
                var notSkipEmpty = importParameter.MemberInfo.MemberTypeInfo.IsPersistent&& (importParameter.Map.ImportStrategy == PersistentTypesImportStrategy.SkipEmpty &&
                                    (columnValue == DBNull.Value || ReferenceEquals(columnValue, string.Empty)));
                var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
                if (!notSkipEmpty && !Imported(columnValue, importParameter,importToObject,objectSpace)){
                    var importResult = new FailedResult{
                        ExcelColumnValue = columnValue?.ToString(),
                        ExcelColumnName = importParameter.Map.ExcelColumnName
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

        public static IMemberInfo GetKeyMember(this ITypeInfo typeInfo) {
            var keyAttribute = typeInfo.FindAttribute<ExcelImportKeyAttribute>();
            return keyAttribute != null ? typeInfo.FindMember(keyAttribute.MemberName) : typeInfo.DefaultMember;
        }

        private static bool Imported(object columnValue,ImportParameter importParameter,  object importToObject,
            IObjectSpace objectSpace){

            var memberTypeInfo = importParameter.MemberInfo.MemberTypeInfo;
            object result;
            if (memberTypeInfo.IsPersistent){
                var keyMember = memberTypeInfo.GetKeyMember();
                if (columnValue.TryToChange(keyMember.MemberType, out result)){
                    try {
                        var type=importParameter.Map.MemberTypeValues.Select(value => value.RegexValue == ".*" ? value.PropertyType :
                            Regex.Match(columnValue.ToString(), value.RegexValue).Success ? value.PropertyType :null).WhereNotNull().First();
                        var referenceObject = GetReferenceObject(objectSpace, importParameter, type, keyMember, result);
                        if (referenceObject != null) {
                            keyMember.SetValue(referenceObject, result);
                            importParameter.MemberInfo.SetValue(importToObject, referenceObject);
                        }
                    }
                    catch (Exception e){
                        Tracing.Tracer.LogError(e);
                        return false;
                    }
                    return true;
                }

                return false;
            }

            var tryToChange = columnValue.TryToChange(importParameter.MemberInfo.MemberType, out result);
            importParameter.MemberInfo.SetValue(importToObject,result);
            return tryToChange;
        }

        private static object GetReferenceObject(IObjectSpace objectSpace, ImportParameter importParameter,
            Type type, IMemberInfo keyMember, object result) {
            var importStrategy = importParameter.Map.ImportStrategy;
            if (importStrategy == PersistentTypesImportStrategy.CreateAlways)
                return objectSpace.CreateObject(type);
            var criteria = CriteriaOperator.Parse($"{keyMember.Name}=?", result);
            var referenceObject = objectSpace.FindObject(type, criteria, true);
            if (importStrategy == PersistentTypesImportStrategy.FailNotFound) {
                if (referenceObject == null)
                    throw new ReferenceObjectNotFoundException(type, criteria);
                return referenceObject;
            }
            if (importStrategy == PersistentTypesImportStrategy.SkipOrCreate) {
                return referenceObject != null ? null : objectSpace.CreateObject(type);
            }
            if (importStrategy == PersistentTypesImportStrategy.UpdateOnly) {
                return referenceObject;
            }
            return referenceObject ?? objectSpace.CreateObject(type);
        }


        public static DataSet GetDataSet(this IExcelDataReader excelDataReader, ExcelImport excelImport){
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

        public static string GetColumnName(this ExcelColumnMap excelColumnMap){
            return !string.IsNullOrWhiteSpace(excelColumnMap.ExcelImport.ColumnMappingRegexPattern)
                ? Regex.Replace(excelColumnMap.ExcelColumnName,
                    excelColumnMap.ExcelImport.ColumnMappingRegexPattern,
                    excelColumnMap.ExcelImport.ColumnMappingReplacement+"")
                : excelColumnMap.ExcelColumnName;
        }

        public static void Map(this ExcelImport excelImport){
            using (var memoryStream = new MemoryStream(excelImport.File.Content)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    using (var dataSet = excelDataReader.GetDataSet(excelImport)){
                        foreach (var dataColumn in dataSet.Tables.Cast<DataTable>()
                            .First(table => table.TableName == excelImport.SheetName).Columns.Cast<DataColumn>()){
                            var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
                            var excelColumnMap = objectSpace.CreateObject<ExcelColumnMap>();
                            excelImport.ExcelColumnMaps.Add(excelColumnMap);
                            excelColumnMap.ExcelColumnName = dataColumn.ColumnName;
                            var member = excelImport.FindMember(excelColumnMap.GetColumnName());
                            if (member != null)
                                excelColumnMap.PropertyName =
                                    CaptionHelper.GetMemberCaption(member.Owner.Type, member.Name);
                        }
                    }
                }
            }
        }

        public struct CalculationProgress{
            public int Progress { get; }
            public string ProgressText { get; }

            public CalculationProgress(int progress, string progressText): this(){
                Progress = progress;
                ProgressText = progressText;
            }
        }

        public static int Import(this ExcelImport excelImport, DataSet dataSet,
            IObserver<ImportProgress> progress = null) {
            var index = 0;
            try {
                var importToTypeInfo = excelImport.Type.GetTypeInfo();
                var columnMembers = GetImportParameters(excelImport, importToTypeInfo);
                var dataRows = dataSet.Tables.Cast<DataTable>().Where(table => table.TableName == excelImport.SheetName).SelectMany(table => table.Rows.Cast<DataRow>()).ToArray();
                var dataRowsLength = dataRows.Length;
                var importProgressStart = new ImportProgressStart(excelImport.Oid){Total=dataRowsLength};
                progress?.OnNext(importProgressStart);
                foreach (var dataRow in dataRows){
                    index++;
                    var percentage = index*100/dataRowsLength;
                    progress?.OnNext(new ImportDataRowProgress(excelImport.Oid,percentage){DataRow=dataRow});
                    var importToObject = GetImportToObject(importToTypeInfo, columnMembers, dataRow, excelImport);
                    if (importToObject != null) {
                        progress?.OnNext(new ImportObjectProgress(excelImport.Oid,percentage){ObjectToImport=importToObject});
                        Import(excelImport, dataRow, importToObject, index, columnMembers);
                    }
                }
            }
            catch (Exception e) {
                progress?.OnNext(new ImportProgressException(excelImport.Oid,e, index, excelImport.FailedResultList.FailedResults.Count));
                throw;
            }
            progress?.OnNext(new ImportProgressComplete(excelImport.Oid,index,0));
            return index;
        }

        private static ImportParameter[] GetImportParameters(ExcelImport excelImport, ITypeInfo importToTypeInfo){
            return excelImport.ExcelColumnMaps.Select(map => {
                var memberInfo = importToTypeInfo.Members.First(info => !string.IsNullOrEmpty(info.DisplayName)
                    ? info.DisplayName == map.PropertyName: info.Name == map.PropertyName);
                return new ImportParameter(){MemberInfo=memberInfo,Map=map};
            }).ToArray();
        }

        public static IObservable<T> Where<T>(this IObservable<T> source, ExcelImport excelImport) where T:ImportProgress{
            return source.Where(progress => progress.ExcelImportKey == excelImport.Oid);
        }

        public static DataSet GetDataSet(this ExcelImport excelImport, byte[] bytes = null) {
            bytes = bytes ?? excelImport.File.Content;
            using (var memoryStream = new MemoryStream(bytes)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)) {
                    return excelDataReader.GetDataSet(excelImport);
                }
            }
        }

        public static int Import(this ExcelImport excelImport,byte[] bytes=null,IObserver<ImportProgress> progress=null){
            excelImport.FailedResultList.FailedResults.Clear();
            int index;
            using (var dataSet = excelImport.GetDataSet(bytes)){
                index = excelImport.Import(dataSet, progress);
            }

            if (!string.IsNullOrWhiteSpace(excelImport.ValidationContexts)) {
                var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
                Validator.RuleSet.ValidateAll(objectSpace, objectSpace.ModifiedObjects,excelImport.ValidationContexts);
            }

            return index;
        }
    }

    public class ReferenceObjectNotFoundException : Exception {
        public CriteriaOperator Criteria{ get; }

        public ReferenceObjectNotFoundException(Type type, CriteriaOperator criteria):base($"{type} for {criteria} not found") {
            Criteria = criteria;
            Type = type;
        }

        public Type Type { get; }


    }
}