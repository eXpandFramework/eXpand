using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System.Linq;

namespace eXpand.ExpressApp.Core
{
    public static class ObjectSpaceExtensions
    {
        public static T FindObject<T>(this ObjectSpace objectSpace, Expression<Func<T,bool>> expression, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            var genericType = typeof(XPQuery<>).MakeGenericType(new[]{objectType});
            
            var xpquery = Activator.CreateInstance(genericType,new[]{objectSpace.Session});
            
            var innderType = typeof(Func<,>).MakeGenericType(new[]{objectType,typeof(bool)});
            var type = typeof(Expression<>).MakeGenericType(new []{innderType});
            var methodInfo = genericType.GetMethod("TransformExpression",new[]{type});
            methodInfo.Invoke(xpquery, new[] {expression});
            
            
            var transformExpression = new XPQuery<T>(objectSpace.Session).TransformExpression(expression);
            return objectSpace.FindObject<T>(transformExpression,persistentCriteriaEvaluationBehavior==PersistentCriteriaEvaluationBehavior.InTransaction?true: false);
        }

        public static T CreateObjectFromInterface<T>(this ObjectSpace objectSpace) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }
    }
}
