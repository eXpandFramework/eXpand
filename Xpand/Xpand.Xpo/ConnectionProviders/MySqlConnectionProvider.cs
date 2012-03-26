using System;
using System.Data;
using System.Globalization;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace Xpand.Xpo.ConnectionProviders {
    public class MySqlConnectionProvider : DevExpress.Xpo.DB.MySqlConnectionProvider {
        public MySqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption) : base(connection, autoCreateOption) {
        }

        public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
            base.GetTableSchema(table, checkIndexes, checkForeignKeys);
            table.Columns.Clear();
            GetColumns(table);
        }

        void GetColumns(DBTable table) {
            foreach (SelectStatementResultRow row in SelectData(new Query(string.Format(CultureInfo.InvariantCulture, "show columns from `{0}`", ComposeSafeTableName(table.Name)))).Rows) {
                int size;
                string rowValue1, rowValue5, rowValue0;
                if (row.Values[1].GetType() == typeof(Byte[])) {
                    rowValue1 = System.Text.Encoding.Default.GetString((byte[])row.Values[1]);
                    rowValue5 = System.Text.Encoding.Default.GetString((byte[])row.Values[5]);
                    rowValue0 = System.Text.Encoding.Default.GetString((byte[])row.Values[0]);
                } else {
                    rowValue1 = (string)row.Values[1];
                    rowValue5 = (string)row.Values[5];
                    rowValue0 = (string)row.Values[0];
                }
                DBColumnType type = GetTypeFromString(rowValue1, out size);
                bool isAutoIncrement = false;
                string extraValue = rowValue5;
                if (!string.IsNullOrEmpty(extraValue) && extraValue.Contains("auto_increment")) isAutoIncrement = true;
                var column = new DBColumn(rowValue0, false, String.Empty, type == DBColumnType.String ? size : 0, type)
                             {IsIdentity = isAutoIncrement};
                table.AddColumn(column);
            }
        }

        DBColumnType GetTypeFromString(string typeName, out int size) {
            size = 0;
            switch (typeName) {
                case "char(1)":
                    return DBColumnType.Char;
                case "tinyint(1)":
                    return DBColumnType.Boolean;
            }
            string typeWithoutBrackets = RemoveBrackets(typeName);
            switch (typeWithoutBrackets) {
                case "int":
                    return DBColumnType.Int32;
                case "int unsigned":
                    return DBColumnType.UInt32;
                case "longblob":
                case "tinyblob":
                case "mediumblob":
                case "blob":
                    return DBColumnType.ByteArray;
                case "text":
                case "mediumtext":
                    return DBColumnType.String;
                case "bit":
                case "tinyint unsigned":
                    return DBColumnType.Byte;
                case "tinyint":
                    return DBColumnType.SByte;
                case "smallint":
                    return DBColumnType.Int16;
                case "smallint unsigned":
                    return DBColumnType.UInt16;
                case "bigint":
                    return DBColumnType.Int64;
                case "bigint unsigned":
                    return DBColumnType.UInt64;
                case "double":
                    return DBColumnType.Double;
                case "datetime":
                case "timestamp":
                    return DBColumnType.DateTime;
                    
            }
            if (typeName.StartsWith("char(")) {
                size = Int32.Parse(typeName.Substring(5, typeName.Length - 6));
                return DBColumnType.String;
            }
            if (typeName.StartsWith("varchar(")) {
                size = Int32.Parse(typeName.Substring(8, typeName.Length - 9));
                return DBColumnType.String;
            }
            return DBColumnType.Unknown;
        }
        static string RemoveBrackets(string typeName) {
            string typeWithoutBrackets = typeName;
            int bracketOpen = typeName.IndexOf('(');
            if (bracketOpen >= 0) {
                int bracketClose = typeName.IndexOf(')', bracketOpen);
                if (bracketClose >= 0) {
                    typeWithoutBrackets = typeName.Remove(bracketOpen, bracketClose - bracketOpen + 1);
                }
            }
            return typeWithoutBrackets;
        }

    }
}