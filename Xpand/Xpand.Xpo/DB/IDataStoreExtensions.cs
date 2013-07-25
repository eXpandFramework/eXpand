using System;
using System.Globalization;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.DB {
    public static class IDataStoreExtensions {
        public static ConnectionProviderSql ConnectionProviderSql(this IDataStore dataStore) {
            var dataStoreProxy = dataStore as DataStoreProxy;
            return dataStoreProxy ?? (ConnectionProviderSql)dataStore;
        }

        public static void CreateColumn(this IDataStore dataStore, XPMemberInfo xpMemberInfo, DBTable table, bool throwOnError=false) {
            dataStore.ConnectionProviderSql().CreateColumn(xpMemberInfo, table,throwOnError);
        }

        public static void CreateColumn(this ConnectionProviderSql connectionProviderSql, XPMemberInfo xpMemberInfo, DBTable table, bool throwOnError) {
            var dbColumnType = GetDbColumnType(xpMemberInfo);
            var column = new DBColumn(xpMemberInfo.Name, false, null, xpMemberInfo.MappingFieldSize, dbColumnType);
            string textSql = String.Format(CultureInfo.InvariantCulture, "alter table {0} add {1} {2}",
                                            connectionProviderSql.FormatTableSafe(table),
                                            connectionProviderSql.FormatColumnSafe(column.Name),
                                            connectionProviderSql.GetSqlCreateColumnFullAttributes(table, column));
            try {
                connectionProviderSql.ExecSql(new Query(textSql));
            } catch (SqlExecutionErrorException) {
                if (throwOnError)
                    throw;
            } catch (SchemaCorrectionNeededException) {
                if (throwOnError)
                    throw;
            }
            
        }

        static DBColumnType GetDbColumnType(XPMemberInfo xpMemberInfo) {
            Type type = xpMemberInfo.StorageType;
            var xpClassInfo = xpMemberInfo.Owner.Dictionary.QueryClassInfo(type);
            if (xpClassInfo != null) {
                type = xpClassInfo.KeyProperty.StorageType;
            }
            return DBColumn.GetColumnType(type);
        }
    }
}
