using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Utils.Linq;

namespace eXpand.ExpressApp.Core
{
    public static class ObjectSpaceExtensions
    {
        public static T FindObject<T>(this ObjectSpace objectSpace, Expression<Func<T,bool>> expression, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            CriteriaOperator criteriaOperator = GetCriteriaOperator(objectType, expression, objectSpace);
            bool inTransaction = persistentCriteriaEvaluationBehavior==PersistentCriteriaEvaluationBehavior.InTransaction?true: false;
            return (T)objectSpace.FindObject(objectType,criteriaOperator,inTransaction) ;
        }

        static CriteriaOperator GetCriteriaOperator<T>(Type objectType, Expression<Func<T, bool>> expression, ObjectSpace objectSpace) {
            var transform = new ExpressionTransformer().Transform(objectType, expression);
            var genericType = typeof(XPQuery<>).MakeGenericType(new[]{objectType});
            var xpquery = Activator.CreateInstance(genericType,new[]{objectSpace.Session});
            var innderType = typeof(Func<,>).MakeGenericType(new[]{objectType,typeof(bool)});
            var type = typeof(Expression<>).MakeGenericType(new []{innderType});
            var methodInfo = genericType.GetMethod("TransformExpression",new[]{type});
            return (CriteriaOperator) methodInfo.Invoke(xpquery, new[] { transform });
        }

        public static T CreateObjectFromInterface<T>(this ObjectSpace objectSpace) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }
    }
}
