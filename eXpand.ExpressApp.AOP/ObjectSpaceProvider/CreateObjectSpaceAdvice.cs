using AopAlliance.Intercept;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public class CreateObjectSpaceAdvice : IMethodInterceptor
    {


        public object Invoke(IMethodInvocation invocation)
        {
            
            var events = ((IObjectSpaceProviderObjectSpaceEvents)invocation.Proxy);
            var objectSpaceProvider = (DevExpress.ExpressApp.ObjectSpaceProvider)invocation.Proxy;
            var args = new ObjectSpaceProviderObjectSpaceArgs(objectSpaceProvider);
            events.InvokeCreating(args);
            if (args.Handled)
                return args.ObjectSpace;
            var proceed = (ObjectSpace) invocation.Proceed();
            var args1 = new ObjectSpaceProviderObjectSpaceArgs((IObjectSpaceProvider) invocation.Target) { ObjectSpace = proceed };
            events.InvokeCreated(args1);
            if (args1.Handled)
            {
                proceed.Dispose();
                return args1.ObjectSpace;
            }
            
            return proceed;
        }
    }
}