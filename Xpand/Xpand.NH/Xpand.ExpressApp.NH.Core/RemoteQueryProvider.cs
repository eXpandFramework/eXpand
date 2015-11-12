using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public class RemoteQueryProvider : IQueryProvider
    {
        private readonly IExpressionExecutor executor;
        private int addSecurityWhereCount;


        public RemoteQueryProvider(IExpressionExecutor executor)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");

            this.executor = executor;
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }


        public void BeginAddSecurityWhere()
        {
            addSecurityWhereCount++;
        }

        public void EndAddSecurityWhere()
        {
            addSecurityWhereCount--;
        }

        private bool CanAddSecurityWhere { get { return addSecurityWhereCount == 0; } }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                var result = (IQueryable)Activator.CreateInstance(typeof(RemoteObjectQuery<>).MakeGenericType(elementType), this, expression );
                if (CanAddSecurityWhere)
                    executor.AddSecurityWhere(result, elementType);
                return result;
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return executor.Execute(expression);
        }
    }
}