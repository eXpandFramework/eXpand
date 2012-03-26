using System;
using System.Data;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace Xpand.Xpo.ConnectionProviders {
    public class OracleConnectionProvider : DevExpress.Xpo.DB.OracleConnectionProvider {
        public OracleConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption) {
        }
        public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
            base.GetTableSchema(table, checkIndexes, checkForeignKeys);
            table.Columns.Clear();
            GetColumns(table);
        }
        void GetColumns(DBTable table) {
            string schema = ComposeSafeSchemaName(table.Name);
            string safeTableName = ComposeSafeTableName(table.Name);
            Query query = schema == string.Empty ? new Query("SELECT COLUMN_NAME, DATA_TYPE, CHAR_COL_DECL_LENGTH, DATA_PRECISION, DATA_SCALE from USER_TAB_COLUMNS where TABLE_NAME = :p0",
                                    new QueryParameterCollection(new OperandValue(safeTableName)), new[] { ":p0" })
                              : new Query("SELECT COLUMN_NAME, DATA_TYPE, CHAR_COL_DECL_LENGTH, DATA_PRECISION, DATA_SCALE from ALL_TAB_COLUMNS where OWNER = :p0 and TABLE_NAME = :p1",
                                    new QueryParameterCollection(new OperandValue(schema), new OperandValue(safeTableName)), new[] { ":p0", ":p1" });
            foreach (SelectStatementResultRow row in SelectData(query).Rows) {
                int size = row.Values[2] != DBNull.Value ? ((IConvertible)row.Values[2]).ToInt32(CultureInfo.InvariantCulture) : 0;
                int precision = row.Values[3] != DBNull.Value ? ((IConvertible)row.Values[3]).ToInt32(CultureInfo.InvariantCulture) : 0;
                int scale = row.Values[4] != DBNull.Value ? ((IConvertible)row.Values[4]).ToInt32(CultureInfo.InvariantCulture) : 0;
                DBColumnType type = GetTypeFromString((string)row.Values[1], size, precision, scale);
                table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? size : 0, type));
            }
        }
        DBColumnType GetTypeFromString(string typeName, int size, int precision, int scale) {
            switch (typeName.ToLower()) {
                case "int":
                    return DBColumnType.Int32;
                case "blob":
                case "raw":
                    return DBColumnType.ByteArray;
                case "number":
                    if (precision == 0 || scale != 0)
                        return DBColumnType.Decimal;
                    if (precision < 3)
                        return DBColumnType.Byte;
                    if (precision < 5)
                        return DBColumnType.Int16;
                    if (precision < 10)
                        return DBColumnType.Int32;
                    if (precision < 20)
                        return DBColumnType.Int64;
                    return DBColumnType.Decimal;
                case "nchar":
                case "char":
                    if (size > 1)
                        return DBColumnType.String;
                    return DBColumnType.Char;
                case "money":
                    return DBColumnType.Decimal;
                case "float":
                    return DBColumnType.Double;
                case "nvarchar":
                case "varchar":
                case "varchar2":
                case "nvarchar2":
                    return DBColumnType.String;
                case "date":
                case "timestamp":
                    return DBColumnType.DateTime;
                case "clob":
                case "nclob":
                    return DBColumnType.String;
            }
            return DBColumnType.Unknown;
        }

    }
}