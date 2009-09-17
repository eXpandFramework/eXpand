using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace eXpand.Xpo
{
    public static class SessionExtensions
    {
        public static int GetCount<ClassType>(this Session session){
            return (int) session.Evaluate<ClassType>(new AggregateOperand("", Aggregate.Count), null);
        }

        public static object GetObject(this Session session, object o){
            if (o== null)
                return null;
            return session.GetObjectByKey(o.GetType(), ((PersistentBase)o).ClassInfo.KeyProperty.GetValue(o));
        }
        public static ClassType FindObject<ClassType>(this Session session,PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior, Expression<Func<ClassType,bool>> expression){
            return (ClassType)
                session.FindObject(persistentCriteriaEvaluationBehavior, typeof (ClassType), new XPQuery<ClassType>(session).TransformExpression(expression));
        }
        public static ClassType FindObject<ClassType>(this Session session, Expression<Func<ClassType,bool>> expression)
        {
            
            return (ClassType) session.FindObject(typeof(ClassType), new XPQuery<ClassType>(session).TransformExpression(expression), false);
        }
    }
}