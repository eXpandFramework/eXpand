using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Linq;
using Xpand.Xpo.DB;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public static class ObjectSpaceExtensions {
        public static bool Contains<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,bool intransaction=true) where T : class{
            return objectSpace.FindObject(expression, intransaction) != null;
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
            var xpObjectSpace = objectSpace as XPObjectSpace;
            if (xpObjectSpace != null) {
                create.Invoke(((BaseDataLayer)xpObjectSpace.Session.DataLayer).ConnectionProvider);
                return;
            }
            throw new NotImplementedException();

        }
        public static bool IsServerSide(this IObjectSpace objectSpace) {
            var xpObjectSpace = objectSpace as XPObjectSpace;
            if (xpObjectSpace != null) {
                var session = xpObjectSpace.Session;
                return (session.DataLayer != null && session.ObjectLayer == null) || (session.DataLayer != null && session.ObjectLayer != null);
            }
            throw new NotImplementedException(objectSpace.GetType().FullName);
        }

        public static void RollBackSilent(this IObjectSpace objectSpace) {
            objectSpace.ConfirmationRequired += (sender, args) => args.ConfirmationResult = ConfirmationResult.No;
            objectSpace.Rollback();
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

        public static IList<TClassType> GetObjects<TClassType>(this IObjectSpace objectSpace, Expression<Func<TClassType, bool>> expression) {
            CriteriaOperator criteriaOperator = new XPQuery<TClassType>(((XPObjectSpace)objectSpace).Session).TransformExpression(expression);
            return objectSpace.GetObjects<TClassType>(criteriaOperator);
        }

        public static bool NeedReload(this IObjectSpace objectSpace, object currentObject) {
            XPMemberInfo optimisticLockFieldInfo;
            XPClassInfo classInfo = GetClassInfo(objectSpace, currentObject, out optimisticLockFieldInfo);
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

        public static T FindObject<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression,bool intransaction){
            return objectSpace.FindObject(expression, intransaction ? PersistentCriteriaEvaluationBehavior.BeforeTransaction : PersistentCriteriaEvaluationBehavior.InTransaction);
        }

        public static T FindObject<T>(this IObjectSpace objectSpace, Expression<Func<T, bool>> expression, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior=PersistentCriteriaEvaluationBehavior.BeforeTransaction) {
            CriteriaOperator criteriaOperator;
            Type objectType=typeof(T);
            if (objectType.IsInterface){
                objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
                criteriaOperator = GetCriteriaOperator(objectType, expression, objectSpace);
            }
            else{
                criteriaOperator=new XPQuery<T>(((XPObjectSpace) objectSpace).Session).TransformExpression(expression);
            }
            bool inTransaction = persistentCriteriaEvaluationBehavior == PersistentCriteriaEvaluationBehavior.InTransaction;
            return (T)objectSpace.FindObject(objectType, criteriaOperator, inTransaction);
        }

        static CriteriaOperator GetCriteriaOperator<T>(Type objectType, Expression<Func<T, bool>> expression, IObjectSpace objectSpace) {
            Expression transform = new ExpressionConverter().Convert(objectType, expression);
            var genericType = typeof(XPQuery<>).MakeGenericType(new[] { objectType });
            var xpquery = genericType.CreateInstance(new object[] { ((XPObjectSpace)objectSpace).Session });
            return (CriteriaOperator) xpquery.CallMethod("TransformExpression", new object[]{transform});
        }

        public static T CreateObjectFromInterface<T>(this IObjectSpace objectSpace) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }
    }
}
