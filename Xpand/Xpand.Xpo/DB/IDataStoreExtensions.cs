using System;
using System.Globalization;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.DB {
    public static class IDataStoreExtensions {
        public static ConnectionProviderSql ConnectionProviderSql(this IDataStore dataStore) {
            var dataStoreProxy = dataStore as DataStoreProxy;
            return dataStoreProxy ?? (ConnectionProviderSql)dataStore;
        }

        public static void CreateColumn(this IDataStore dataStore, XPMemberInfo xpMemberInfo,DBTable table) {
            dataStore.ConnectionProviderSql().CreateColumn(xpMemberInfo, table);
        }

        public static void CreateColumn(this ConnectionProviderSql connectionProviderSql, XPMemberInfo xpMemberInfo, DBTable table) {
            var dbColumnType = DBColumn.GetColumnType(xpMemberInfo.StorageType);
            var column = new DBColumn(xpMemberInfo.Name, false, null, xpMemberInfo.MappingFieldSize, dbColumnType);
            string textSql = String.Format(CultureInfo.InvariantCulture, "alter table {0} add {1} {2}",
                                            connectionProviderSql.FormatTableSafe(table),
                                            connectionProviderSql.FormatColumnSafe(column.Name),
                                            connectionProviderSql.GetSqlCreateColumnFullAttributes(table, column));
            connectionProviderSql.ExecSql(new Query(textSql));
        }

    }
}
