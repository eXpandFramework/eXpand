using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.ConnectionProviders {
    public class MSSqlConnectionProvider : DevExpress.Xpo.DB.MSSqlConnectionProvider {
        public const string GetDayAndMonth = "GetDayAndMonth";
        public MSSqlConnectionProvider(string connection, AutoCreateOption autoCreateOption)
            : this(new SqlConnection(connection), autoCreateOption) {
        }

        public MSSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption) {
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