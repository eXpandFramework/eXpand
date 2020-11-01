using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.DB;
using Fasterflect;
using Xpand.Utils.Linq;
using Xpand.Xpo.ConnectionProviders;
using MSSqlConnectionProvider = DevExpress.Xpo.DB.MSSqlConnectionProvider;
using MySqlConnectionProvider = DevExpress.Xpo.DB.MySqlConnectionProvider;
using OracleConnectionProvider = DevExpress.Xpo.DB.OracleConnectionProvider;

namespace Xpand.Persistent.Base.General {
    public static class ObjectSpaceProviderExtensions {
        public static void MakeThreadSafe(this IObjectSpaceProvider objectSpaceProvider) {
            objectSpaceProvider.SetFieldValue("threadSafe", true);
        }
    }
    public static class ObjectSpaceExtensions {

        public static ConnectionProviderType GetProviderType(this IObjectSpaceProvider provider) {
            var helper = new ConnectionStringParser(provider.ConnectionString+"");
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == MySqlConnectionProvider.XpoProviderTypeString)
                return ConnectionProviderType.MySQL;
            if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
                return ConnectionProviderType.MSSQL;
            if (providerType == OracleConnectionProvider.XpoProviderTypeString)
                return ConnectionProviderType.Oracle;
            return ConnectionProviderType.Unknown;
        }

        public static IObjectSpaceProvider FindProvider(this IList<IObjectSpaceProvider> providers,Type type){
            return (providers.Select(objectSpaceProvider
                => new { objectSpaceProvider, originalObjectType = objectSpaceProvider.EntityStore.GetOriginalType(type) })
                .Where(t => (t.originalObjectType != null) && t.objectSpaceProvider.EntityStore.RegisteredEntities.Contains(t.originalObjectType))
                .Select(t => t.objectSpaceProvider)).FirstOrDefault();
        }


        [DebuggerStepThrough]
        public static void SetIsModified(this IObjectSpace objectSpace,bool isModified){
            objectSpace.CallMethod("SetIsModified", new[] { typeof(bool) }, isModified);
        }

        public static CriteriaOperator TransformExpression<T>(this IObjectSpace objectSpace,Expression<Func<T, bool>> expression){
            return new XPQuery<T>(objectSpace.Session()).TransformExpression(expression);
        }

        public static void RollbackSilent(this IObjectSpace objectSpace){
            objectSpace.CallMethod("Reload");
        }

        [DebuggerStepThrough]
        public static Session Session(this IObjectSpace objectSpace){
            return ((XPObjectSpace) objectSpace).Session;
        }
        [DebuggerStepThrough]
        public static UnitOfWork UnitOfWork(this IObjectSpace objectSpace){
            return (UnitOfWork) ((XPObjectSpace) objectSpace).Session;
        }

        public static T GetObject<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,
            bool intransaction = true) where T : class{
            return objectSpace.QueryObject(expression, intransaction) ?? objectSpace.CreateObject<T>();
        }

        public static bool Contains<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,bool intransaction=true) where T : class{
            return objectSpace.QueryObject(expression, intransaction) != null;
        }

        public static bool Contains<T>(this IObjectSpace objectSpace) where T:class {
            return objectSpace.Contains<T>(null);
        }

        public static XPClassInfo FindXPClassInfo(this IObjectSpace objectSpace, Type type) {
            ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(type);
            var xpoTypeInfoSource = ((XpoTypeInfoSource) objectSpace.GetFieldValue("xpoTypeInfoSource"));
            if (xpoTypeInfoSource.TypeIsKnown(typeInfo.Type)) {
                return xpoTypeInfoSource.GetEntityClassInfo(typeInfo.Type);
            }
            return null;
        }

        public static Type GetObjectKeyType(this IObjectSpace objectSpace, Type objectType) {
            
            Type result = null;
            var xpClassInfo = objectSpace.FindXPClassInfo(objectType);
            if (xpClassInfo != null) {
                Type queryableType = xpClassInfo.ClassType;
                if (queryableType.IsInterface) {
                    queryableType = PersistentInterfaceHelper.GetPersistentInterfaceDataType(queryableType);
                    xpClassInfo = ((XPObjectSpace) objectSpace).Session.GetClassInfo(queryableType);
                }
                XPMemberInfo keyMember = xpClassInfo.KeyProperty;
                if (keyMember != null) {
                    if (!keyMember.IsStruct) {
                        if (keyMember.ReferenceType != null) {
                            result = objectSpace.GetObjectKeyType(keyMember.ReferenceType.ClassType);
                        } else {
                            return keyMember.MemberType;
                        }
                    } else {
                        throw new NotImplementedException();
                    }
                }
            }
            return result;
        }

        public static void CreateForeignKey(this IObjectSpace objectSpace, XPCustomMemberInfo customMemberInfo,bool throwUnableToCreateDBObjectException = false) {
            CreateDbObject(objectSpace,store => store.CreateForeignKey(customMemberInfo, throwUnableToCreateDBObjectException));
        }

        public static void CreateColumn(this IObjectSpace objectSpace, XPCustomMemberInfo customMemberInfo,  bool throwUnableToCreateDBObjectException = false) {
            CreateDbObject(objectSpace, store => store.CreateColumn(customMemberInfo, throwUnableToCreateDBObjectException));
        }

        private static void CreateDbObject(IObjectSpace objectSpace,Action<IDataStore> create) {
            if (objectSpace is XPObjectSpace xpObjectSpace) {
                create.Invoke(((BaseDataLayer)xpObjectSpace.Session.DataLayer).ConnectionProvider);
                return;
            }
            throw new NotImplementedException();

        }
        public static bool IsServerSide(this IObjectSpace objectSpace) {
            if (objectSpace is XPObjectSpace xpObjectSpace) {
                var session = xpObjectSpace.Session;
                return (session.DataLayer != null && session.ObjectLayer == null) || (session.DataLayer != null && session.ObjectLayer != null);
            }
            throw new NotImplementedException(objectSpace.GetType().FullName);
        }

        public static IEnumerable<TClassType> GetNonDeletedObjectsToSave<TClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<TClassType>().Where(type => !(objectSpace.IsDeletedObject(type)));
        }
        public static IEnumerable<TClassType> GetDeletedObjectsToSave<TClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<TClassType>().Where(type => (objectSpace.IsDeletedObject(type)));
        }

        public static IEnumerable<TClassType> GetNewObjectsToSave<TClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<TClassType>().Where(type => objectSpace.IsNewObject(type));
        }
        public static IEnumerable<TClassType> GetObjectsToUpdate<TClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<TClassType>().Where(type => !objectSpace.IsNewObject(type));
        }

        public static IEnumerable<TClassType> QueryObjects<TClassType>(this IObjectSpace objectSpace){
            return objectSpace.QueryObjects<TClassType>(PersistentCriteriaEvaluationBehavior.InTransaction);
        }

        public static IEnumerable<TClassType> QueryObjects<TClassType>(this IObjectSpace objectSpace,PersistentCriteriaEvaluationBehavior behavior){
            return objectSpace.QueryObjects<TClassType>(null, behavior);
        }

        public static IEnumerable<TClassType> QueryObjects<TClassType>(this IObjectSpace objectSpace, Expression<Func<TClassType, bool>> expression,PersistentCriteriaEvaluationBehavior behavior=PersistentCriteriaEvaluationBehavior.InTransaction){
            var objectType = objectSpace.TypesInfo.FindBussinessObjectType<TClassType>();
            var criteriaOperator = objectSpace.GetCriteriaOperator(expression);
            return objectSpace.GetObjects(objectType,criteriaOperator,behavior==PersistentCriteriaEvaluationBehavior.InTransaction).Cast<TClassType>();
        }

        public static bool NeedReload(this IObjectSpace objectSpace, object currentObject) {
            XPClassInfo classInfo = GetClassInfo(objectSpace, currentObject, out var optimisticLockFieldInfo);
            Boolean isObjectChangedByAnotherUser = false;
            if (!objectSpace.IsDisposedObject(currentObject) && !objectSpace.IsNewObject(currentObject) && (optimisticLockFieldInfo != null)) {
                Object keyPropertyValue = objectSpace.GetKeyValue(currentObject);
                Object lockFieldValue = optimisticLockFieldInfo.GetValue(currentObject);

                if (lockFieldValue != null) {
                    if (((XPObjectSpace) objectSpace).Session.FindObject(currentObject.GetType(), new GroupOperator(
                            new BinaryOperator(objectSpace.GetKeyPropertyName(currentObject.GetType()), keyPropertyValue),
                            new BinaryOperator(classInfo.OptimisticLockFieldName, lockFieldValue)), true) == null) {
                        isObjectChangedByAnotherUser = true;
                    }
                } else {
                    if (((XPObjectSpace)objectSpace).Session.FindObject(currentObject.GetType(), new GroupOperator(
                            new BinaryOperator(objectSpace.GetKeyPropertyName(currentObject.GetType()), keyPropertyValue),
                            new NullOperator(classInfo.OptimisticLockFieldName)), true) == null) {
                        isObjectChangedByAnotherUser = true;
                    }
                }
            }
            return isObjectChangedByAnotherUser;
        }
        private static XPClassInfo FindObjectXPClassInfo(Object obj, Session session) {
            return session.Dictionary.QueryClassInfo(obj);
        }

        static XPClassInfo GetClassInfo(this IObjectSpace objectSpace, object currentObject, out XPMemberInfo optimisticLockFieldInfo) {
            XPClassInfo classInfo = FindObjectXPClassInfo(currentObject, ((XPObjectSpace)objectSpace).Session);
            optimisticLockFieldInfo = classInfo.OptimisticLockFieldInDataLayer;
            return classInfo;
        }

        public static T QueryObject<T>(this Session session, Expression<Func<T, bool>> expression,
            Type implType,
            bool intransaction = true){
            var criteriaOperator = session.GetCriteriaOperator(expression);
            return (T)session.FindObject(implType, criteriaOperator, intransaction);
        }

        public static T QueryObject<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,Type implType,
            bool intransaction = true){
            return objectSpace.Session().QueryObject(expression, implType);
        }

        public static T QueryObject<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,bool intransaction=true){
            var objectType = objectSpace.TypesInfo.FindBussinessObjectType<T>();
            return objectSpace.QueryObject(expression, objectType, intransaction);
        }

        public static T Create<T>(this Session session){
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T) objectType.CreateInstance(session);
        }

        public static T Create<T>(this IObjectSpace objectSpace){
            return typeof(T).IsInterface
                ? (T) objectSpace.CreateObject(objectSpace.TypesInfo.FindBussinessObjectType<T>())
                : objectSpace.CreateObject<T>();
        }

        public static CriteriaOperator GetCriteriaOperator<T>(this Session session,
            Expression<Func<T, bool>> expression){
            if (expression != null) {
                var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
                if (typeof(T).IsInterface) {
                    var tranform = ExpressionConverter.Tranform(expression, objectType);
                    var genericType = typeof(XPQuery<>).MakeGenericType(objectType);
                    var xpquery = Activator.CreateInstance(genericType, session);
                    return (CriteriaOperator)xpquery.CallMethod("TransformExpression", tranform);
                }
                return new XPQuery<T>(session).TransformExpression(expression);
            }
            return null;

        }

        public static CriteriaOperator GetCriteriaOperator<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression){
            return objectSpace.Session().GetCriteriaOperator(expression);
        }

    }
}
