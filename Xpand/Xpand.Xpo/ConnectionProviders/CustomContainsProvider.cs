using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace Xpand.Xpo.ConnectionProviders {
    public class MSSqlConnectionProvider : DevExpress.Xpo.DB.MSSqlConnectionProvider {
        public const string GetDayAndMonth = "GetDayAndMonth";
        public MSSqlConnectionProvider(string connection, AutoCreateOption autoCreateOption)
            : this(new SqlConnection(connection), autoCreateOption) {
        }

        public MSSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption) {
        }
        public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
            base.GetTableSchema(table, checkIndexes, checkForeignKeys);
            table.Columns.Clear();
            GetColumns(table);
        }
        void GetColumns(DBTable table) {
            string schema = ComposeSafeSchemaName(table.Name);
            Query query = schema == string.Empty ? new Query("select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @p1",
                                    new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new[] { "@p1" })
                              : new Query("select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @p1 and TABLE_SCHEMA = @p2",
                                    new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new[] { "@p1", "@p2" });
            foreach (SelectStatementResultRow row in SelectData(query).Rows) {
                int size = row.Values[2] != DBNull.Value ? ((IConvertible)row.Values[2]).ToInt32(CultureInfo.InvariantCulture) : 0;
                DBColumnType type = GetTypeFromString((string)row.Values[1], size);
                table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? size : 0, type));
            }
        }

        DBColumnType GetTypeFromString(string typeName, int length) {
            switch (typeName) {
                case "int":
                    return DBColumnType.Int32;
                case "image":
                case "varbinary":
                    return DBColumnType.ByteArray;
                case "nchar":
                case "char":
                    if (length == 1)
                        return DBColumnType.Char;
                    return DBColumnType.String;
                case "varchar":
                case "nvarchar":
                case "xml":
                case "ntext":
                case "text":
                    return DBColumnType.String;
                case "bit":
                    return DBColumnType.Boolean;
                case "tinyint":
                    return DBColumnType.Byte;
                case "smallint":
                    return DBColumnType.Int16;
                case "bigint":
                    return DBColumnType.Int64;
                case "numeric":
                case "decimal":
                    return DBColumnType.Decimal;
                case "money":
                case "smallmoney":
                    return DBColumnType.Decimal;
                case "float":
                    return DBColumnType.Double;
                case "real":
                    return DBColumnType.Single;
                case "uniqueidentifier":
                    return DBColumnType.Guid;
                case "datetime":
                case "timestamp":
                case "datetime2":
                case "smalldatetime":
                case "date":
                    return DBColumnType.DateTime;
            }
            return DBColumnType.Unknown;
        }

        protected override UpdateSchemaResult ProcessUpdateSchema(bool skipIfFirstTableNotExists,
                                                                  params DBTable[] tables) {
            bool weStartedTran = false;
            if (CanCreateSchema) {
                if (Transaction == null) {
                    ExplicitBeginTransaction();
                    weStartedTran = true;
                }
            }
            try {
                var result = base.ProcessUpdateSchema(skipIfFirstTableNotExists, tables);

                if (result == UpdateSchemaResult.SchemaExists)
                    UpdateColumnSize(tables);

                if (weStartedTran)
                    ExplicitCommitTransaction();

                return result;
            } catch {
                if (weStartedTran)
                    ExplicitRollbackTransaction();
                throw;
            }
        }

        private void UpdateColumnSize(IEnumerable<DBTable> tables) {
            foreach (var table in tables) {
                DBTable actualTable = null;
                foreach (var column in from col in table.Columns where col.ColumnType == DBColumnType.String select col) {
                    if (actualTable == null) {
                        actualTable = new DBTable(table.Name);
                        GetTableSchema(actualTable, false, false);
                    }

                    DBColumn dbColumn = column;
                    var actualColumn = actualTable.Columns.Find(col => string.Compare(col.Name, ComposeSafeColumnName(dbColumn.Name), true) == 0);
                    if ((actualColumn != null) &&
                        (actualColumn.ColumnType == DBColumnType.String) &&
                        (actualColumn.Size != column.Size) &&
                        (column.DBTypeName != string.Format("varchar({0})", actualColumn.Size))) {
                        if ((actualColumn.Size < column.Size) || (column.Size == DevExpress.Xpo.SizeAttribute.Unlimited)) {
                            ExecuteSqlSchemaUpdate("Column",
                                                   column.Name,
                                                   table.Name,
                                                   string.Format(CultureInfo.InvariantCulture,
                                                                 "alter table {0} alter column {1} {2}",
                                                                 FormatTableSafe(table),
                                                                 FormatColumnSafe(column.Name),
                                                                 GetSqlCreateColumnFullAttributes(table, column)));
                        } else
                            System.Diagnostics.Debug.Fail("The size of a DB column will not be decreased." +
                                                          " So changing the SizeAttribute of a column to have a smaller size than previously specified will have no effect.");
                    }
                }
            }
        }

        public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
            if (operatorType == FunctionOperatorType.Custom) {
                if (operands[0] == "GetDayAndMonth") {
                    const string format = "reverse(substring(reverse('0'+rtrim(cast(DATEPART({1}, {0}) as CHAR(2)))),0,3))";
                    string formatMonth = string.Format(format, operands[1], "month");
                    string formatDay = string.Format(format, operands[1], "day");
                    return string.Format("{0}+'/'+{1}", formatDay, formatMonth);
                }
                if (operands.Length == 3) {
                    string format = String.Format("{0}({1}, {2})", operands[0], operands[1], operands[2]);
                    return format;
                }
                if (operands.Length == 2) {
                    string format = String.Format("{0}({1})", operands[0], operands[1]);
                    return format;
                }
            }
            return base.FormatFunction(operatorType, operands);
        }
    }
}