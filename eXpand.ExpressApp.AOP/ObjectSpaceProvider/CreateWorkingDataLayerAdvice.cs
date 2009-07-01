using AopAlliance.Intercept;
using DevExpress.Xpo.DB;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public class CreateWorkingDataLayerAdvice : IMethodInterceptor
    {
        


        public object Invoke(IMethodInvocation invocation)
        {
            var events = ((IObjectSpaceProviderDataStoreEvents)invocation.Proxy);
            var store = invocation.Arguments[0];
            var args = new ObjectSpaceProviderDataStoreArgs((IDataStore) store);
            events.InvokeCreating(args);
            object proceed = args.Handled ? invocation.Method.Invoke(invocation.Target, new object[] {args.DataStore}) : invocation.Proceed();
            events.InvokeCreated(new ObjectSpaceProviderDataStoreArgs((IDataStore)store));
            return proceed;
        }
    }
}