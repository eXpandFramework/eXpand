using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Linq;

namespace Xpand.Persistent.Base.General {
    public static class ObjectSpaceExtensions {
        public static IEnumerable<ClassType> GetNonDeletedObjectsToSave<ClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<ClassType>().Where(type => !(objectSpace.IsDeletedObject(type)));
        }
        public static IEnumerable<ClassType> GetDeletedObjectsToSave<ClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<ClassType>().Where(type => (objectSpace.IsDeletedObject(type)));
        }

        public static IEnumerable<ClassType> GetNewObjectsToSave<ClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<ClassType>().Where(type => objectSpace.IsNewObject(type));
        }
        public static IEnumerable<ClassType> GetObjectsToUdate<ClassType>(this IObjectSpace objectSpace) {
            return objectSpace.GetObjectsToSave(true).OfType<ClassType>().Where(type => !objectSpace.IsNewObject(type));
        }

        public static IList<ClassType> GetObjects<ClassType>(this IObjectSpace objectSpace, Expression<Func<ClassType, bool>> expression) {
            CriteriaOperator criteriaOperator = new XPQuery<ClassType>(((ObjectSpace)objectSpace).Session).TransformExpression(expression);
            return objectSpace.GetObjects<ClassType>(criteriaOperator);
        }

        public static bool NeedReload(this ObjectSpace objectSpace, object currentObject) {
            XPMemberInfo optimisticLockFieldInfo;
            XPClassInfo classInfo = GetClassInfo(objectSpace, currentObject, out optimisticLockFieldInfo);
            Boolean isObjectChangedByAnotherUser = false;
            if (!objectSpace.IsDisposedObject(currentObject) && !objectSpace.IsNewObject(currentObject) && (optimisticLockFieldInfo != null)) {
                Object keyPropertyValue = objectSpace.GetKeyValue(currentObject);
                Object lockFieldValue = optimisticLockFieldInfo.GetValue(currentObject);

                if (lockFieldValue != null) {
                    if (objectSpace.Session.FindObject(currentObject.GetType(), new GroupOperator(
                            new BinaryOperator(objectSpace.GetKeyPropertyName(currentObject.GetType()), keyPropertyValue),
                            new BinaryOperator(classInfo.OptimisticLockFieldName, lockFieldValue)), true) == null) {
                        isObjectChangedByAnotherUser = true;
                    }
                } else {
                    if (objectSpace.Session.FindObject(currentObject.GetType(), new GroupOperator(
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

        static XPClassInfo GetClassInfo(this ObjectSpace objectSpace, object currentObject, out XPMemberInfo optimisticLockFieldInfo) {
            XPClassInfo classInfo = FindObjectXPClassInfo(currentObject, objectSpace.Session);
            optimisticLockFieldInfo = classInfo.OptimisticLockFieldInDataLayer;
            return classInfo;
        }

        public static T FindObject<T>(this ObjectSpace objectSpace, Expression<Func<T, bool>> expression, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            CriteriaOperator criteriaOperator = GetCriteriaOperator(objectType, expression, objectSpace);
            bool inTransaction = persistentCriteriaEvaluationBehavior == PersistentCriteriaEvaluationBehavior.InTransaction ? true : false;
            return (T)objectSpace.FindObject(objectType, criteriaOperator, inTransaction);
        }

        static CriteriaOperator GetCriteriaOperator<T>(Type objectType, Expression<Func<T, bool>> expression, ObjectSpace objectSpace) {
            Expression transform = new ExpressionConverter().Convert(objectType, expression);
            var genericType = typeof(XPQuery<>).MakeGenericType(new[] { objectType });
            var xpquery = Activator.CreateInstance(genericType, new[] { objectSpace.Session });
            var innderType = typeof(Func<,>).MakeGenericType(new[] { objectType, typeof(bool) });
            var type = typeof(Expression<>).MakeGenericType(new[] { innderType });
            var methodInfo = genericType.GetMethod("TransformExpression", new[] { type });
            return (CriteriaOperator)methodInfo.Invoke(xpquery, new[] { transform });
        }

        public static T CreateObjectFromInterface<T>(this IObjectSpace objectSpace) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }
    }
}
