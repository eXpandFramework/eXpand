using System;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace Xpand.Xpo.Collections
{
    public static class XPCollectionExtensions
    {
        public static void CreateCriteria<T>(this XPCollection<T> collection, Expression<Func<T, bool>> expression)
        {
            collection.Criteria = new XPQuery<T>(collection.Session).TransformExpression(expression);
        }
        public static void CombineFilter<T>(this XPCollection<T> collection, Expression<Func<T, bool>> expression)
        {
            collection.Filter = XPBaseCollectionExtensions.CombineCriteria(collection.Filter,
                                                                           new XPQuery<T>(collection.Session).TransformExpression(expression));
        }

    }
}