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
using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.Metadata.Helpers;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;
using Xpand.Utils.Linq;
using IObjectSpaceLink = DevExpress.ExpressApp.IObjectSpaceLink;
using Type = System.Type;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    public static class ImportExtension{

        public static IMemberInfo FindMember(this ExcelColumnMap excelColumnMap) {
            return excelColumnMap.ExcelImport?.FindMember(excelColumnMap.PropertyName);
        }

        public static IMemberInfo FindMember(this ExcelImport excelImport,string member) {
            return excelImport?.Type.GetTypeInfo().Members.WhereMapable().FirstOrDefault(info =>info.IsMatch( member));
        }

        public static string Caption(this IMemberInfo info) {
            return CaptionHelper.GetMemberCaption(info);
        }

        public static bool IsMatch(this IMemberInfo info,string member){
            return CaptionHelper.GetMemberCaption(info).Equals(member, StringComparison.OrdinalIgnoreCase);
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

        private static object GetImportToObject(ImportParameter[] importParameters, DataRow dataRow,
            ExcelImport excelImport, int index,IObjectSpace failedResultsObjectSpace, Func<(Type objectType, CriteriaOperator criteria),object> func){
            var importToTypeInfo = excelImport.Type.GetTypeInfo();
            var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
            if (excelImport.ImportStrategy==ImportStrategy.CreateAlways)
                return objectSpace.CreateObject(importToTypeInfo.Type);
            var valueTuple = importParameters.FirstOrDefault(tuple => {
                var keyMember = importToTypeInfo.GetKeyMember(excelImport);
                return keyMember != null && tuple.ModelMember.MemberInfo == keyMember;
            });
            if (valueTuple.IsDefault())
                throw new KeyMemberNotMappedException(importToTypeInfo.Type);
            var columnValue = dataRow[valueTuple.Map.ExcelColumnName];
            var criteria = CriteriaOperator.Parse($"{valueTuple.ModelMember.Name}=?", columnValue);
            var importToObject = func((importToTypeInfo.Type,criteria));
            if (excelImport.ImportStrategy == ImportStrategy.UpdateOnly) {
                return importToObject;
            }
            if (excelImport.ImportStrategy == ImportStrategy.FailNotFound) {
                if (importToObject == null) {
                    var item = failedResultsObjectSpace.CreateObject<FailedImportResult>();
                    item.ExcelColumnValue = $"{columnValue}";
                    item.Index = index;
                    item.Reason = $"{nameof(ImportStrategy)}.{nameof(ImportStrategy.FailNotFound)} with criteria:{criteria}";
                    excelImport.FailedResults.Add(item);
                }
                return importToObject;
            }
            if (excelImport.ImportStrategy==ImportStrategy.UpdateOrCreate){
                return importToObject ?? objectSpace.CreateObject(importToTypeInfo.Type);
            }
            if (excelImport.ImportStrategy==ImportStrategy.SkipOrCreate)
                return importToObject != null ? null : objectSpace.CreateObject(importToTypeInfo.Type);
            throw new NotImplementedException(excelImport.ImportStrategy.ToString());
        }

        public struct ImportParameter {
            public IModelMember ModelMember { get; set; }
            public ExcelColumnMap Map { get; set; }
            
        }
        private static List<FailedImportResult> Import(ExcelImport excelImport, DataRow dataRow, object importToObject, int index,
            ImportParameter[] importParameterses, IObjectSpace failedResultsObjectSpace) {

            var results=new List<FailedImportResult>();
            foreach (var importParameter in importParameterses){
                var columnValue = dataRow[importParameter.Map.ExcelColumnName];
                var skipEmpty = importParameter.ModelMember.MemberInfo.MemberTypeInfo.IsPersistent&& (importParameter.Map.SkipEmpty &&
                                    (columnValue == DBNull.Value || ReferenceEquals(columnValue, string.Empty)));
                if (!skipEmpty ){
                    var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
                    var failedResult = Import(columnValue, importParameter, importToObject, objectSpace,failedResultsObjectSpace,failedResultsObjectSpace.GetObject(excelImport));
                    if (failedResult!=null) {
                        results.Add(failedResult);
                    }
                }
            }

            foreach (var failedResult in results){
                var importResultImportedObject = importToObject.ToString();
                failedResult.ImportedObject = importResultImportedObject;
                failedResult.Index = index;
                excelImport.FailedResults.Add(excelImport.ObjectSpace.GetObject(failedResult));
            }

            return results;
        }

        public static IMemberInfo GetKeyMember(this ITypeInfo typeInfo,ExcelImport excelImport) {
            var keyAttribute = typeInfo.FindAttribute<ExcelImportKeyAttribute>();
            if (keyAttribute != null)
                return typeInfo.FindMember(keyAttribute.MemberName);
            var excelImportKey = excelImport.KeyMaps.SelectMany(_ => _.Keys).FirstOrDefault(_ => _.Type==typeInfo.Type);
            return excelImportKey != null ? typeInfo.FindMember(excelImportKey.Property) : typeInfo.DefaultMember;
        }

        private static FailedImportResult Import(object columnValue, ImportParameter importParameter,
            object importToObject,IObjectSpace objectSpace, IObjectSpace failedResultsObjectSpace,ExcelImport excelImport){
            
            var memberTypeInfo = importParameter.Map.GetObjectType(columnValue).GetTypeInfo();
            object result;
            
            if (memberTypeInfo.IsPersistent){
                var keyMember = memberTypeInfo.GetKeyMember(excelImport);
                if (columnValue.TryToChange(keyMember.MemberType, out result)){
                    try {
                        var referenceObject = GetReferenceObject(objectSpace, importParameter,  keyMember, result);
                        if (referenceObject != null) {
                            keyMember.SetValue(referenceObject, result);
                            importParameter.ModelMember.MemberInfo.SetValue(importToObject, referenceObject);
                        }
                    }
                    catch (Exception e){
                        Tracing.Tracer.LogError(e);
                        var failedResult = NewFailedImportResult(columnValue, importParameter, importToObject, failedResultsObjectSpace,excelImport);
                        failedResult.Reason = e.Message;
                        return failedResult;
                    }
                    return null;
                }
            }

            var tryToChange = columnValue.TryToChange(memberTypeInfo.Type, out result);
            if (!tryToChange) {
                var failedResult = NewFailedImportResult(columnValue, importParameter, importToObject, failedResultsObjectSpace,excelImport);
                failedResult.Reason = $"Cannot convert colum value to {memberTypeInfo.Type}";
                return failedResult;
            }

            if (importParameter.ModelMember.MemberInfo.MemberType == typeof(string) && $"{columnValue}".Length > importParameter.ModelMember.Size) {
                var failedResult = NewFailedImportResult(columnValue, importParameter, importToObject, failedResultsObjectSpace,excelImport);
                failedResult.Reason = "Size constrain";
                return failedResult;
            }

            if (importParameter.ModelMember.MemberInfo.FindAttributes<KeyAttribute>().Any() && result == null) {
                var failedResult = NewFailedImportResult(columnValue, importParameter, importToObject, failedResultsObjectSpace,excelImport);
                failedResult.Reason = "Null value for Key";
                return failedResult;
            }
            importParameter.ModelMember.MemberInfo.SetValue(importToObject,result);
            return null;
        }

        private static FailedImportResult NewFailedImportResult(object columnValue, ImportParameter importParameter,
            object importToObject, IObjectSpace failedResultsObjectSpace,ExcelImport excelImport){
            var failedResult = failedResultsObjectSpace.CreateObject<FailedImportResult>();
            failedResult.ExcelColumnName = importParameter.Map.ExcelColumnName;
            failedResult.ExcelColumnValue = $"{columnValue}";
            failedResult.ImportedObject = $"{importToObject}";
            failedResult.ExcelImport=excelImport;
            return failedResult;
        }

        private static object GetReferenceObject(IObjectSpace objectSpace, ImportParameter importParameter,
             IMemberInfo keyMember, object columnValue) {
            var importStrategy = importParameter.Map.ImportStrategy;
            if (importStrategy == PersistentTypesImportStrategy.CreateAlways)
                return CreateObject(objectSpace, importParameter,columnValue);
            var criteria =columnValue==null?new NullOperator(keyMember.Name) : CriteriaOperator.Parse($"{keyMember.Name}=?", columnValue);
            var type = importParameter.Map.GetObjectType(columnValue);
            var referenceObject = objectSpace.FindObject(type, criteria, true);
            if (importStrategy == PersistentTypesImportStrategy.FailNotFound) {
                if (referenceObject == null)
                    throw new ReferenceObjectNotFoundException($"{CaptionHelper.GetClassCaption(type.FullName)}: {importParameter.ModelMember.Caption}", criteria);
                return referenceObject;
            }
            if (importStrategy == PersistentTypesImportStrategy.UpdateOnly) {
                return referenceObject;
            }

            return referenceObject ?? CreateObject(objectSpace, importParameter, columnValue);
        }

        private static object CreateObject(IObjectSpace objectSpace, ImportParameter importParameter, object columnValue){
            var type = importParameter.Map.GetObjectType(columnValue);
            return objectSpace.CreateObject(type);
        }

        public static Type GetObjectType(this ExcelColumnMap map,object columnValue){
            var type = map.MemberTypeValues.Select(value => value.RegexValue == ".*" ? value.PropertyType :
                    Regex.Match(columnValue.ToString(), value.RegexValue).Success ? value.PropertyType : null).WhereNotNull()
                .FirstOrDefault();
            if (type == null) {
                throw new InvalidOperationException($"Cannot match {nameof(ExcelColumnMap.ExcelColumnName)} {map.ExcelColumnName} against column value: {columnValue}");
            }
            return type;
        }


        public static DataSet GetDataSet(this IExcelDataReader excelDataReader, ExcelImport excelImport){
            var excelDataSetConfiguration = new ExcelDataSetConfiguration{
                ConfigureDataTable = reader => new ExcelDataTableConfiguration(){
                    UseHeaderRow = excelImport.HeaderRows>0,
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

        public static string GetColumnName(this ExcelColumnMap excelColumnMap) {
            if (excelColumnMap.ExcelColumnName == null)
                return null;
            return !string.IsNullOrWhiteSpace(excelColumnMap.ExcelImport.ColumnMappingRegexPattern)
                ? Regex.Replace(excelColumnMap.ExcelColumnName,
                    excelColumnMap.ExcelImport.ColumnMappingRegexPattern,
                    excelColumnMap.ExcelImport.ColumnMappingReplacement+"")
                : excelColumnMap.ExcelColumnName;
        }

        public static void Map(this ExcelImport excelImport) {
            Validator.RuleSet.Validate(((IObjectSpaceLink) excelImport).ObjectSpace, excelImport,ExcelImport.MappingContext);
            using (var memoryStream = new MemoryStream(excelImport.File.Content)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)) {
                    foreach (var dataColumn in excelDataReader.Columns(excelImport)) {
                        var objectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace;
                        var excelColumnMap = objectSpace.CreateObject<ExcelColumnMap>();
                        excelImport.ExcelColumnMaps.Add(excelColumnMap);
                        excelColumnMap.ExcelColumnName = dataColumn;
                        var member = excelImport.FindMember(excelColumnMap.GetColumnName());
                        if (member != null)
                            excelColumnMap.PropertyName =CaptionHelper.GetMemberCaption(member.Owner.Type, member.Name);
                    }
                }                
            }
        }

        public static IEnumerable<string> Sheets(this IExcelDataReader reader) {
            do {
                yield return reader.Name;
            } while (reader.NextResult());
        }

        public static IEnumerable<string> Columns(this IExcelDataReader reader,ExcelImport excelImport) {
            return reader.Sheets().SelectMany(s => {
                int row = 0;
                while (excelImport.HeaderRows > row) {
                    row++;
                    reader.Read();
                }

                return GetColumnNames(reader, excelImport);
            });

        }

        private static IEnumerable<string> GetColumnNames(IExcelDataReader reader, ExcelImport excelImport){
            for (int i = 0; i < reader.FieldCount; i++){
                if (excelImport.HeaderRows > 0)
                    yield return reader.GetString(i);
                else{
                    yield return  $"{i + 1}";
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

        public static int Import(this ExcelImport excelImport, DataTable dataTable,IObjectSpace failResultsObjectSpace,
            IObserver<ImportProgress> progress = null,ImportParameter[] importParameters=null) {
            var index = 0;
            try {
                var failedImportResults = failResultsObjectSpace.GetObjectsQuery<FailedImportResult>().Where(_ => _.ExcelImport.Oid==excelImport.Oid).ToArray();
                failResultsObjectSpace.Delete(failedImportResults);
                failResultsObjectSpace.CommitChanges();
                if (progress != null) {
                    var importProgressStart = new ImportProgressStart(excelImport.Oid) {DataTable = dataTable};
                    progress.OnNext(importProgressStart);
                }

                if (dataTable == null) {
                    throw new InvalidOperationException($"Sheet {excelImport.SheetName} not found");
                }

                importParameters = importParameters ?? excelImport.GetImportParameters();

                foreach (var dataRow in dataTable.Rows.Cast<DataRow>()) {
                    index++;
                    var percentage = index * 100 / dataTable.Rows.Count;
                    progress?.OnNext(new ImportDataRowProgress(excelImport.Oid, percentage) {DataRow = dataRow});
                    var importToObject = GetImportToObject(importParameters, dataRow, excelImport, index,
                        failResultsObjectSpace, _ => {
                            var requestImportTargetObject = new RequestImportTargetObject(_, excelImport.Oid) {
                                ObjectSpace = ((IObjectSpaceLink) excelImport).ObjectSpace
                            };
                            progress?.OnNext(requestImportTargetObject);
                            return requestImportTargetObject.TargetObject ??
                                   ((IObjectSpaceLink) excelImport).ObjectSpace.FindObject(_.objectType, _.criteria);
                        });
                    if (importToObject != null) {
                        progress?.OnNext(new ImportObjectProgress(excelImport.Oid, percentage){ObjectToImport = importToObject});
                        var failedResults = Import(excelImport, dataRow, importToObject, index, importParameters, failResultsObjectSpace);
                        if (failedResults.Any()&&excelImport.AbortThreshold>0) {
                            if (excelImport.FailedResults.GroupBy(_ => _.Index).Count() * 100 / dataTable.Rows.Count >excelImport.AbortThreshold) {
                                throw new InvalidOperationException($"{excelImport.Name} {nameof(excelImport.AbortThreshold)} reached.");
                            }
                        }
                    }

                    if (index % 10 == 0) {
                        excelImport.ObjectSpace.CommitChanges();
                    }
                }

            }
            catch (Exception e) {
                progress?.OnNext(new ImportProgressException(excelImport.Oid, e, index,
                    new BindingList<FailedImportResult>(excelImport.FailedResults)));
                Tracing.Tracer.LogError(e);
                return index;
            }
            finally {
                failResultsObjectSpace.CommitChanges();
            }
            progress?.OnNext(new ImportProgressComplete(excelImport.Oid,index,new BindingList<FailedImportResult>(excelImport.FailedResults)));
            return index;
        }

        public static ImportParameter[] GetImportParameters(this ExcelImport excelImport){
            return excelImport.ExcelColumnMaps.Select(map => {
                var modelClass = CaptionHelper.ApplicationModel.BOModel.GetClass(excelImport.Type);
                var modelMember = modelClass.AllMembers.FirstOrDefault(_ => _.Caption == map.PropertyName);
                if (modelMember==null)
                    throw new MemberNotFoundException(excelImport.Type,map.PropertyName);
                return new ImportParameter(){ModelMember=modelMember,Map=map};
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

        public static void ValidateForImport(this ExcelImport excelImport) {
            var ruleSet = Validator.RuleSet;
            var objects = new[] {excelImport}.Cast<object>().Concat(excelImport.ExcelColumnMaps)
                .Concat(excelImport.ExcelColumnMaps.SelectMany(map => map.MemberTypeValues)).ToArray();

            ruleSet.ValidateAll(((IObjectSpaceLink) excelImport).ObjectSpace,objects ,ExcelImport.ImportingContext);
        }

        public static int Import(this ExcelImport excelImport,IObjectSpace failResultsObjectSpace,byte[] bytes=null,IObserver<ImportProgress> progress=null,ImportParameter[] importParameters=null) {
            int index;
            using (var dataSet = excelImport.GetDataSet(bytes)){
                var dataTable = dataSet.Tables.Cast<DataTable>().FirstOrDefault(table => table.TableName == excelImport.SheetName);
                index = excelImport.Import(dataTable,failResultsObjectSpace, progress,importParameters);
            }

            return index;
        }
    }

    [Serializable]
    public class KeyMemberNotMappedException : Exception {
        public Type Type{ get; }

        public KeyMemberNotMappedException(Type type) {
            Type = type;
        }
    }

    [Serializable]
    public class ReferenceObjectNotFoundException : Exception {
        public CriteriaOperator Criteria{ get; }

        public ReferenceObjectNotFoundException(string caption, CriteriaOperator criteria):base($"{caption} for {criteria} not found") {
            Criteria = criteria;
            Caption = caption;
        }

        public string Caption { get; }


    }
}