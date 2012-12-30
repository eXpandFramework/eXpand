using System;
using System.Data;
using System.Data.OleDb;

namespace Xpand.Utils.Helpers {
    public enum ExcelExtension {
        XLS,
        XLSX
    }

    public static class DataSetExtensions {
        public static void ImportExcelXLS(this DataSet output, string[] fileNames, ExcelExtension excelExtension = ExcelExtension.XLS, bool hasHeaders = true) {
            foreach (var fileName in fileNames) {
                var strConn = ConnectionString(fileName, hasHeaders, excelExtension);
                using (var conn = new OleDbConnection(strConn)) {
                    conn.Open();
                    DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    ImportExcelXLS(output, fileName, schemaTable, conn);
                }
            }
        }

        static string ConnectionString(string fileName, bool hasHeaders, ExcelExtension excelExtension) {
            string HDR = hasHeaders ? "Yes" : "No";
            fileName = string.Format("{0}.{1}", fileName, excelExtension);
            return excelExtension == ExcelExtension.XLSX
                       ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\""
                       : "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";
        }

        static void ImportExcelXLS(DataSet output, string fileName, DataTable schemaTable, OleDbConnection conn) {
            if (schemaTable != null)
                foreach (DataRow schemaRow in schemaTable.Rows) {
                    string sheet = schemaRow["TABLE_NAME"].ToString();
                    if (!sheet.EndsWith("_") && !sheet.EndsWith("$")) {
                        try {
                            var cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn) {
                                CommandType = CommandType.Text
                            };
                            var outputTable = new DataTable(sheet);
                            output.Tables.Add(outputTable);
                            new OleDbDataAdapter(cmd).Fill(outputTable);
                        } catch (Exception ex) {
                            throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, fileName), ex);
                        }
                    }
                }
        }
    }
}