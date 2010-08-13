using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Core
{
    public static class ObjectSpaceExtensions
    {
        public static T FindObject<T>(this ObjectSpace objectSpace, Expression<Func<T,bool>> expression, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
//            new XPQuery<T>(objectSpace.Session).TransformExpression()
            var lambdaExpression = MethodName<T>(expression);
            var genericType = typeof(XPQuery<>).MakeGenericType(new[]{objectType});
            var xpquery = Activator.CreateInstance(genericType,new[]{objectSpace.Session});
            
            var innderType = typeof(Func<,>).MakeGenericType(new[]{objectType,typeof(bool)});
            var type = typeof(Expression<>).MakeGenericType(new []{innderType});
            var methodInfo = genericType.GetMethod("TransformExpression",new[]{type});
            methodInfo.Invoke(xpquery, new[] {lambdaExpression});
            
            
            var transformExpression = new XPQuery<T>(objectSpace.Session).TransformExpression(expression);
            return objectSpace.FindObject<T>(transformExpression,persistentCriteriaEvaluationBehavior==PersistentCriteriaEvaluationBehavior.InTransaction?true: false);
        }
        private static LambdaExpression MethodName<T>(Expression<Func<T, bool>> expression)
        {
            Type persistentAttributeInfoType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
ParameterExpression parameter = Expression.Parameter(persistentAttributeInfoType, "info");
LambdaExpression lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(persistentAttributeInfoType, typeof(bool)), expression.Body,
    parameter);
return lambda;
        }
        public static T CreateObjectFromInterface<T>(this ObjectSpace objectSpace) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }
    }
}
