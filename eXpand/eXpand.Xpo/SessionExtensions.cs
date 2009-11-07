using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo
{
    public static class SessionExtensions
    {
        public static int GetCount(this Session session,Type type) {
            return (int) session.Evaluate(type, new AggregateOperand("", Aggregate.Count), null);
        }

        public static int GetCount<ClassType>(this Session session){
            return (int) session.Evaluate<ClassType>(new AggregateOperand("", Aggregate.Count), null);
        }

        public static object GetObject(this Session session,PersistentCriteriaEvaluationBehavior behavior, object o){
            if (o == null)
                return null;
            XPMemberInfo property = ((PersistentBase) o).ClassInfo.KeyProperty;
            return session.FindObject(behavior, o.GetType(), new BinaryOperator(property.Name, property.GetValue(o)));
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