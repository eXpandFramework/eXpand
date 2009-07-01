using System;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace eXpand.Xpo
{
    public static class SessionExtensions
    {
        public static ClassType FindObject<ClassType>(this Session session, Expression<Func<ClassType,bool>> expression)
        {
            return (ClassType) session.FindObject(typeof(ClassType), new XPQuery<ClassType>(session).TransformExpression(expression), false);
        }
    }
}