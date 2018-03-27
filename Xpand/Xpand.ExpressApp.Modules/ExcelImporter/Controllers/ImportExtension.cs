using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public static class ImportExtension{
        private static object GetImportToObject(ITypeInfo importToTypeInfo,
            (string column, IMemberInfo memberInfo)[] columnNames, DataRow dataRow, ExcelImport excelImport){
            if (excelImport.ImportStrategy==ImportStrategy.CreateAlways)
                return excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
            var valueTuple = columnNames.First(tuple => tuple.memberInfo==importToTypeInfo.DefaultMember);
            var o = dataRow[valueTuple.column];
            var importToObject = excelImport.ObjectSpace.FindObject(importToTypeInfo.Type,CriteriaOperator.Parse($"{valueTuple.memberInfo.Name}=?", o));
            if (excelImport.ImportStrategy==ImportStrategy.UpdateOrCreate){
                return importToObject ?? excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
            }

            return importToObject != null ? null : excelImport.ObjectSpace.CreateObject(importToTypeInfo.Type);
        }

        private static void Import(ExcelImport excelImport, DataRow dataRow,
            object importToObject, int index, IEnumerable<(string column, IMemberInfo memberInfo)> columnMembers){

            var results=new List<FailedResult>();
            foreach (var columnMember in columnMembers){
                var columnValue = dataRow[columnMember.column];
                if (!Imported(columnValue, columnMember.memberInfo,importToObject,excelImport.ObjectSpace)){
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

        private static bool Imported(object columnValue, IMemberInfo memberInfo, object importToObject,
            IObjectSpace objectSpace){
            var memberTypeInfo = memberInfo.MemberTypeInfo;
            object result;
            if (memberTypeInfo.IsPersistent){
                var type = memberTypeInfo.Type;
                if (columnValue.TryToChange(memberTypeInfo.DefaultMember.MemberType, out result)){
                    try{
                        var referenceObject =objectSpace.FindObject(type,CriteriaOperator.Parse($"{memberTypeInfo.DefaultMember.Name}=?", result), true) ??
                                             objectSpace.CreateObject(type);
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

        public static int Import(this ExcelImport excelImport,byte[] bytes=null){
            bytes = bytes ?? excelImport.File.Content;
            excelImport.FailedResultList.FailedResults.Clear();
            using (var memoryStream = new MemoryStream(bytes)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(memoryStream)){
                    int index;
                    using (var dataSet = excelDataReader.GetDataSet(excelImport)){
                        index = 0;
                        var importToTypeInfo = excelImport.Type.GetTypeInfo();
                        var columnMembers = excelImport.ExcelColumnMaps.Select(map => (column: map.ExcelColumnName,
                            memberInfo: importToTypeInfo.Members.First(info => !string.IsNullOrEmpty(info.DisplayName)
                                ? info.DisplayName == map.PropertyName
                                : info.Name == map.PropertyName))).ToArray();
                        
                        foreach (var dataRow in dataSet.Tables.Cast<DataTable>().First().Rows.Cast<DataRow>()){
                            index++;
                            var importToObject = GetImportToObject(importToTypeInfo, columnMembers, dataRow, excelImport);
                            if (importToObject != null)
                                Import(excelImport, dataRow, importToObject, index, columnMembers);
                        }
                    }

                    return index;
                }
            }
        }

    }
}