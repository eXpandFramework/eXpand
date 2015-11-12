using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public interface IExpressionExecutor
    {
        object Execute(System.Linq.Expressions.Expression expression);
        void AddSecurityWhere(IQueryable queryable, Type elementType);
    }
}
