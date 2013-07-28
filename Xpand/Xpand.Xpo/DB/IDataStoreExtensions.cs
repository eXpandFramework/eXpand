using System;
using System.Collections.Specialized;
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.DB {
    public static class IDataStoreExtensions {
        public static ConnectionProviderSql ConnectionProviderSql(this IDataStore dataStore) {
            var dataStoreProxy = dataStore as DataStoreProxy;
            return dataStoreProxy ?? (ConnectionProviderSql)dataStore;
        }

        public static void CreateForeignKey(this IDataStore dataStore, XPMemberInfo xpMemberInfo,bool throwUnableToCreateDBObjectException = false) {
            dataStore.ConnectionProviderSql().CreateForeignKey(xpMemberInfo,throwUnableToCreateDBObjectException);
        }

        public static void CreateColumn(this IDataStore dataStore, XPMemberInfo xpMemberInfo,bool throwUnableToCreateDBObjectException=false) {
            dataStore.ConnectionProviderSql().CreateColumn(xpMemberInfo);
        }

        public static void CreateForeignKey(this ConnectionProviderSql connectionProviderSql, XPMemberInfo xpMemberInfo,
                                        bool throwUnableToCreateDBObjectException = false) {
            if (xpMemberInfo.HasAttribute(typeof(AssociationAttribute))) {
                CallSchemaUpdateMethod(connectionProviderSql, CreateForeighKey(xpMemberInfo), throwUnableToCreateDBObjectException);
            }
        }

        public static void CreateColumn(this ConnectionProviderSql connectionProviderSql, XPMemberInfo xpMemberInfo, bool throwUnableToCreateDBObjectException = false) {
            var dbColumnType = GetDbColumnType(xpMemberInfo);
            var column = new DBColumn(xpMemberInfo.Name, false, null, xpMemberInfo.MappingFieldSize, dbColumnType);
            CallSchemaUpdateMethod(connectionProviderSql,  sql => sql.CreateColumn(xpMemberInfo.Owner.Table, column),throwUnableToCreateDBObjectException);
            connectionProviderSql.CreateForeignKey(xpMemberInfo,throwUnableToCreateDBObjectException);
        }

        static Action<ConnectionProviderSql> CreateForeighKey(XPMemberInfo xpMemberInfo) {
            return sql => {
                var dbForeignKey = new DBForeignKey(new StringCollection { xpMemberInfo.Name }, xpMemberInfo.ReferenceType.TableName, new StringCollection { xpMemberInfo.ReferenceType.KeyProperty.Name });
                sql.CreateForeignKey(xpMemberInfo.Owner.Table, dbForeignKey);
            };
        }

        static void CallSchemaUpdateMethod(ConnectionProviderSql connectionProviderSql, Action<ConnectionProviderSql> action, bool throwUnableToCreateDBObjectException ) {
            var autoCreateOption = connectionProviderSql.AutoCreateOption;
            var fieldInfo = typeof (DataStoreBase).GetField("_AutoCreateOption", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null) throw new NullReferenceException("fieldInfo");
            fieldInfo.SetValue(connectionProviderSql, AutoCreateOption.SchemaOnly);
            try {
                action.Invoke(connectionProviderSql);
            }
            catch (UnableToCreateDBObjectException) {
                if (throwUnableToCreateDBObjectException)
                    throw;
            }
            finally {
                fieldInfo.SetValue(connectionProviderSql, autoCreateOption);
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

    public interface ICanCreateSchema {
        bool CanCreateSchema { get; }
        void CreateColumn(DBTable table, DBColumn column);
    }

}
