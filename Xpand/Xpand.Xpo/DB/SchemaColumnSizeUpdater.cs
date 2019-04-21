
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using Fasterflect;

namespace Xpand.Xpo.DB {
    public class SchemaColumnSizeUpdater : ISchemaUpdater {
        static SchemaColumnSizeUpdater() {
            Disabled = true;
        }

        static readonly HashSet<string> HashSet=new HashSet<string>();
        public static bool Disabled { get; set; }

        public void Update(ConnectionProviderSql connectionProviderSql, DataStoreUpdateSchemaEventArgs dataStoreUpdateSchemaEventArgs) {
            if (connectionProviderSql == null || connectionProviderSql is AccessConnectionProvider||Disabled)
                return;
            using (((IDisposable) ((AsyncLockHelper) connectionProviderSql.GetPropertyValue("LockHelper")).CallMethod("Lock"))) {
                if (!connectionProviderSql.CanCreateSchema)
                    return;

                try {
                    if (dataStoreUpdateSchemaEventArgs.UpdateSchemaResult == UpdateSchemaResult.SchemaExists) {
                        UpdateColumnSize(dataStoreUpdateSchemaEventArgs.Tables, connectionProviderSql);
                    }
                } catch (Exception e) {
                    Trace.TraceError(e.ToString());
                }
            }
        }

        private void UpdateColumnSize(IEnumerable<DBTable> tables, ConnectionProviderSql sqlDataStore) {
            foreach (var table in tables.Where(table => !HashSet.Contains(table.Name))) {
                HashSet.Add(table.Name);
                DBTable actualTable = null;
                foreach (var column in table.Columns.Where(col => col.ColumnType == DBColumnType.String)) {
                    if (actualTable == null) {
                        actualTable = new DBTable(table.Name);
                        sqlDataStore.GetTableSchema(actualTable, false, false);
                    }
                    DBColumn dbColumn = column;
                    var actualColumn = actualTable.Columns.Find(col => String.Compare(col.Name, sqlDataStore.ComposeSafeColumnName(dbColumn.Name), StringComparison.OrdinalIgnoreCase) == 0);
                    if (NeedsAltering(column, actualColumn)) {
                        if ((actualColumn.Size < column.Size) || (column.Size == SizeAttribute.Unlimited)) {
                            var sql = GetSql(table, sqlDataStore, column);
                            Trace.WriteLineIf(new TraceSwitch("XPO", "").TraceInfo, sql);
                            sqlDataStore.ExecSql(new Query(sql));
                        }
                    }
                }
            }
        }
        string GetSql(DBTable table, ConnectionProviderSql sqlDataStore, DBColumn column) {
            return string.Format(CultureInfo.InvariantCulture,
                                 "alter table {0} {3} {1} {2}",
                                 sqlDataStore.FormatTableSafe(table),
                                 sqlDataStore.FormatColumnSafe(column.Name),
                                 sqlDataStore.GetSqlCreateColumnFullAttributes(table, column,true),
                                 (sqlDataStore is BaseOracleConnectionProvider || sqlDataStore is MySqlConnectionProvider) ? "modify" : "alter column");
        }



        bool NeedsAltering(DBColumn column, DBColumn actualColumn) {
            return (actualColumn != null) &&
                   (actualColumn.ColumnType == DBColumnType.String) &&
                   (actualColumn.Size != column.Size) &&
                   (column.DBTypeName != $"varchar({actualColumn.Size})");
        }

    }
}
