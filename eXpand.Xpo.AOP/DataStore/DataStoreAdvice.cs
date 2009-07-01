using AopAlliance.Intercept;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.AOP.DataStore
{
    public class DataStoreAdvice : IMethodInterceptor
    {
        public object Invoke(IMethodInvocation invocation)
        {
            var e = new StatementArgs((BaseStatement[])invocation.Arguments[0]);
            if (invocation.Method.Name == "SelectData")
                ((IDataStoreEvents) invocation.Proxy).InvokeSelect(e);
            else if (invocation.Method.Name == "ModifyData")
                ((IDataStoreEvents) invocation.Proxy).InvokeModify(e);

            return Proceed(invocation);
        }

        public object Proceed(IMethodInvocation invocation)
        {
            return invocation.Proceed();
        }


    }
}