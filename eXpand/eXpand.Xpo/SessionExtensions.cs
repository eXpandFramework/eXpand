using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo
{
    public static class SessionExtensions
    {
        public static void UnDelete(this XPBaseObject simpleObject) {
            simpleObject.Session.PurgeDeletedObjects();
        }
        public static bool IsNewObject(this IXPSimpleObject simpleObject)
        {
            return simpleObject.Session.IsNewObject(simpleObject);
        }
        public static int GetCount(this Session session, Type type,CriteriaOperator criteriaOperator) {
            return (int)session.Evaluate(type, new AggregateOperand("", Aggregate.Count), criteriaOperator);
        }

        public static int GetCount(this Session session,Type type) {
            return GetCount(session,type, null);
        }

        public static int GetCount<ClassType>(this Session session, CriteriaOperator criteriaOperator)
        {
            return (int)session.Evaluate<ClassType>(new AggregateOperand("", Aggregate.Count), criteriaOperator);
        }

        public static int GetCount<ClassType>(this Session session){
            return GetCount<ClassType>(session, null);
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
        
        public static object FindObject<ClassType>(this Session session, Type classType, Expression<Func<ClassType,bool>> expression, bool selectDeleted)
        {
            return session.FindObject(classType, new XPQuery<ClassType>(session).TransformExpression(expression),
                                      selectDeleted);
        }
        public static ClassType FindObject<ClassType>(this Session session, Expression<Func<ClassType,bool>> expression)
        {
            return (ClassType) session.FindObject(typeof(ClassType), new XPQuery<ClassType>(session).TransformExpression(expression), false);
        }
    }
}