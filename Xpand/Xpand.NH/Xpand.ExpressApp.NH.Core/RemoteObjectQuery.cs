using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    [DataContract, System.Serializable]
    public class RemoteObjectQuery<T> : IQueryable<T>
    {

        private readonly IQueryProvider provider;

        public RemoteObjectQuery()  
        {

        }
        public RemoteObjectQuery(RemoteQueryProvider provider)
            : this(provider, null)
        {
            Expression = Expression.Constant(this);
        }

        public RemoteObjectQuery(RemoteQueryProvider provider, Expression expression)
        {
            this.provider = provider;
            Expression = expression;
        }


        public Type ElementType
        {
            get { return typeof(T); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get;
            private set;
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }

        private IEnumerable result;

        public System.Collections.IEnumerator GetEnumerator()
        {
            if (result == null)
                result = Provider.Execute(Expression) as IEnumerable;
            if (result != null)
                return result.GetEnumerator();
            else
                return null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }
}