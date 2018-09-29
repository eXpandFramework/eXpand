using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public static class ImportExtension{
        private static object GetImportToObject(ITypeInfo importToTypeInfo,
            (string column, IMemberInfo memberInfo, PersistentTypesImportStrategy ImportStrategy)[] columnNames, DataRow dataRow, ExcelImport excelImport){
            if (excelImport.ImportStrategy==ImportStrategy.CreateAlways)
                return excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
            var valueTuple = columnNames.First(tuple => tuple.memberInfo==importToTypeInfo.GetKeyMember());
            var o = dataRow[valueTuple.column];
            var importToObject = excelImport.ObjectSpace.FindObject(importToTypeInfo.Type,CriteriaOperator.Parse($"{valueTuple.memberInfo.Name}=?", o));
            if (excelImport.ImportStrategy == ImportStrategy.UpdateOnly) {
                return importToObject;
            }
            if (excelImport.ImportStrategy==ImportStrategy.UpdateOrCreate){
                return importToObject ?? excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
            }
            return importToObject != null ? null : excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
        }

        private static void Import(ExcelImport excelImport, DataRow dataRow,
            object importToObject, int index, (string column, IMemberInfo memberInfo, PersistentTypesImportStrategy ImportStrategy)[] columnMembers){

            var results=new List<FailedResult>();
            foreach (var columnMember in columnMembers){
                var columnValue = dataRow[columnMember.column];
                var notSkipEmpty = columnMember.memberInfo.MemberTypeInfo.IsPersistent&& (columnMember.ImportStrategy == PersistentTypesImportStrategy.SkipEmpty &&
                                    (columnValue == DBNull.Value || ReferenceEquals(columnValue, string.Empty)));
                if (!notSkipEmpty && !Imported(columnValue, columnMember.memberInfo,importToObject,excelImport.ObjectSpace,columnMember.ImportStrategy)){
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

        public static IMemberInfo GetKeyMember(this ITypeInfo typeInfo) {
            var keyAttribute = typeInfo.FindAttribute<ExcelImportKeyAttribute>();
            return keyAttribute != null ? typeInfo.FindMember(keyAttribute.MemberName) : typeInfo.DefaultMember;
        }

        private static bool Imported(object columnValue, IMemberInfo memberInfo, object importToObject,
            IObjectSpace objectSpace, PersistentTypesImportStrategy importStrategy){
            var memberTypeInfo = memberInfo.MemberTypeInfo;
            object result;
            if (memberTypeInfo.IsPersistent){
                var type = memberTypeInfo.Type;
                var keyMember = memberTypeInfo.GetKeyMember();
                if (columnValue.TryToChange(keyMember.MemberType, out result)){
                    try{
                        var referenceObject = GetReferenceObject(objectSpace, importStrategy, type, keyMember, result);
                        if (referenceObject != null) {
                            keyMember.SetValue(referenceObject, result);
                            memberInfo.SetValue(importToObject, referenceObject);
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

            var tryToChange = columnValue.TryToChange(memberInfo.MemberType, out result);
            memberInfo.SetValue(importToObject,result);
            return tryToChange;
        }

        private static object GetReferenceObject(IObjectSpace objectSpace, PersistentTypesImportStrategy importStrategy,
            Type type, IMemberInfo keyMember, object result) {
            if (importStrategy == PersistentTypesImportStrategy.CreateAlways)
                return objectSpace.CreateObject(type);
            var criteria = CriteriaOperator.Parse($"{keyMember.Name}=?", result);
            var referenceObject = objectSpace.FindObject(type, criteria, true);
            if (importStrategy == PersistentTypesImportStrategy.FailEmpty) {
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

        private static string GetColumnName(ExcelColumnMap excelColumnMap){
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
                            var excelColumnMap = excelImport.ObjectSpace.CreateObject<ExcelColumnMap>();
                            excelImport.ExcelColumnMaps.Add(excelColumnMap);
                            excelColumnMap.ExcelColumnName = dataColumn.ColumnName;
                            excelColumnMap.PropertyName =
                                excelImport.TypePropertyNames.FirstOrDefault(s =>
                                    s.ToLower() == GetColumnName(excelColumnMap).ToLower());
                            
                        }
                    }
                }
            }
        }

        public static int Import(this ExcelImport excelImport,byte[] bytes=null){
            bytes = bytes ?? excelImport.File.Content;
            excelImport.FailedResultList.FailedResults.Clear();
            using (var memoryStream = new MemoryStream(bytes)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    int index;
                    using (var dataSet = excelDataReader.GetDataSet(excelImport)){
                        index = 0;
                        var importToTypeInfo = excelImport.Type.GetTypeInfo();
                        var columnMembers = excelImport.ExcelColumnMaps.Select(map => {
                            var memberInfo = importToTypeInfo.Members.First(info => !string.IsNullOrEmpty(info.DisplayName)
                                ? info.DisplayName == map.PropertyName: info.Name == map.PropertyName);
                            return (column: map.ExcelColumnName, memberInfo,map.ImportStrategy);
                        }).ToArray();
                        
                        foreach (var dataRow in dataSet.Tables.Cast<DataTable>().Where(table => table.TableName == excelImport.SheetName).SelectMany(table => table.Rows.Cast<DataRow>())){
                            index++;
                            var importToObject = GetImportToObject(importToTypeInfo, columnMembers, dataRow, excelImport);
                            if (importToObject != null)
                                Import(excelImport, dataRow, importToObject, index, columnMembers);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(excelImport.ValidationContexts))
                        Validator.RuleSet.ValidateAll(excelImport.ObjectSpace, excelImport.ObjectSpace.ModifiedObjects,excelImport.ValidationContexts);
                    return index;
                }
            }
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