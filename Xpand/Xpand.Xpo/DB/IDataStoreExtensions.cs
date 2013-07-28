using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.DB {
    public static class ProxyGeneratorExtensions {
        public static TInterface MockObject<TInterface>(this object target) where TInterface : class {
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<TInterface>(
                new DuckTypingInterceptor(target,typeof(TInterface),interception => interception.Invocation.ReturnValue = true));
        }
    }
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
    public class DuckTypingInterceptor : IInterceptor {
        private readonly object target;
        readonly Action<Interception> _action;

        public class Interception {
            readonly IInvocation _invocation;
            readonly MethodInfo _methodInfo;

            public Interception(IInvocation invocation, MethodInfo methodInfo) {
                _invocation = invocation;
                _methodInfo = methodInfo;
            }

            public object ReturnObject { get; set; }

            public IInvocation Invocation {
                get { return _invocation; }
            }

            public MethodInfo MethodInfo {
                get { return _methodInfo; }
            }
        }

        readonly HashSet<string>  _names=new HashSet<string>();
        public DuckTypingInterceptor(object target,Type interfaceType, Action<Interception> action) {
            this.target = target;
            var propertyNames = interfaceType.GetProperties().Where(info => info.GetCustomAttributes(typeof (InterceptAttribute), false).Any()).Select(info => info.Name);
            var methodNmeas = interfaceType.GetMethods().Where(info => info.GetCustomAttributes(typeof (InterceptAttribute), false).Any()).Select(info => info.Name);
            foreach (var source in propertyNames.Union(methodNmeas)) {
                _names.Add(source);
            }
            _action = action;
        }

        public void Intercept(IInvocation invocation) {
            var methods = target.GetType().GetMethods()
                .Where(m => m.Name == invocation.Method.Name)
                .Where(m => m.GetParameters().Length == invocation.Arguments.Length)
                .ToList();
            if (methods.Count > 1)
                throw new ApplicationException(string.Format("Ambiguous method match for '{0}'", invocation.Method.Name));
            if (methods.Count == 0)
                throw new ApplicationException(string.Format("No method '{0}' found", invocation.Method.Name));
            var method = methods[0];
            if (invocation.GenericArguments != null && invocation.GenericArguments.Length > 0)
                method = method.MakeGenericMethod(invocation.GenericArguments);
            var interception = new Interception(invocation, method);
            if (!_names.Contains(method.Name))
                invocation.ReturnValue = method.Invoke(target, invocation.Arguments);
            else
                _action.Invoke(interception);
        }
    }

    public sealed class InterceptAttribute:Attribute {
    }

//    public static class DuckType {
//        private static readonly ProxyGenerator generator = new ProxyGenerator();
//
//        public static T As<T>(object o) where T : class {
//            return generator.CreateInterfaceProxyWithoutTarget<T>(new DuckTypingInterceptor(o));
//        }
//    }

//    public class DuckTypingInterceptor : IInterceptor {
//        private readonly object target;
//
//        public DuckTypingInterceptor(object target) {
//            this.target = target;
//        }
//
//        public void Intercept(IInvocation invocation) {
//            var methods = target.GetType().GetMethods()
//                .Where(m => m.Name == invocation.Method.Name)
//                .Where(m => m.GetParameters().Length == invocation.Arguments.Length)
//                .ToList();
//            if (methods.Count > 1)
//                throw new ApplicationException(string.Format("Ambiguous method match for '{0}'", invocation.Method.Name));
//            if (methods.Count == 0)
//                throw new ApplicationException(string.Format("No method '{0}' found", invocation.Method.Name));
//            var method = methods[0];
//            if (invocation.GenericArguments != null && invocation.GenericArguments.Length > 0)
//                method = method.MakeGenericMethod(invocation.GenericArguments);
//            invocation.ReturnValue = method.Invoke(target, invocation.Arguments);
//        }
//    }
}
