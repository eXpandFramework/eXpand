
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace Xpand.Xpo.DB {
    public class SchemaColumnSizeUpdater : ISchemaUpdater {
        public void Update(ConnectionProviderSql connectionProviderSql, DataStoreUpdateSchemaEventArgs dataStoreUpdateSchemaEventArgs) {
            lock (connectionProviderSql.SyncRoot)
            {
                if (!connectionProviderSql.CanCreateSchema)
                    return;

                try
                {
                    if (dataStoreUpdateSchemaEventArgs.UpdateSchemaResult == UpdateSchemaResult.SchemaExists)
                        UpdateColumnSize(dataStoreUpdateSchemaEventArgs.Tables, connectionProviderSql);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Trace.TraceError(e.ToString());
                }
            }
        }

        private void UpdateColumnSize(IEnumerable<DBTable> tables, ConnectionProviderSql sqlDataStore) {
            foreach (var table in tables) {
                DBTable actualTable = null;
                foreach (var column in table.Columns.Where(col => col.ColumnType == DBColumnType.String)) {
                    if (actualTable == null) {
                        actualTable = new DBTable(table.Name);
                        sqlDataStore.GetTableSchema(actualTable, false, false);
                    }
                    DBColumn dbColumn = column;
                    var actualColumn = actualTable.Columns.Find(col => string.Compare(col.Name, sqlDataStore.ComposeSafeColumnName(dbColumn.Name), true) == 0);
                    if (NeedsAltering(column, actualColumn)) {
                        if ((actualColumn.Size < column.Size) || (column.Size == DevExpress.Xpo.SizeAttribute.Unlimited)) {
                            var sql = GetSql(table, sqlDataStore, column);
                            System.Diagnostics.Trace.WriteLineIf(new System.Diagnostics.TraceSwitch("XPO", "").TraceInfo, sql);
                            sqlDataStore.ExecSql(new Query(sql));
                        } else {
                            //                            Debug.Fail("The size of a DB column will not be decreased." +
                            //                                       " So changing the SizeAttribute of a column to have a smaller size than previously specified will have no effect.");
                        }
                    }
                }
            }
        }

        string GetSql(DBTable table, ConnectionProviderSql sqlDataStore, DBColumn column) {
            return string.Format(CultureInfo.InvariantCulture,
                                 "alter table {0} alter column {1} {2}",
                                 sqlDataStore.FormatTableSafe(table),
                                 sqlDataStore.FormatColumnSafe(column.Name),
                                 sqlDataStore.GetSqlCreateColumnFullAttributes(table, column));
        }

        bool NeedsAltering(DBColumn column, DBColumn actualColumn) {
            return (actualColumn != null) &&
                   (actualColumn.ColumnType == DBColumnType.String) &&
                   (actualColumn.Size != column.Size) &&
                   (column.DBTypeName != string.Format("varchar({0})", actualColumn.Size));
        }

    }
}
