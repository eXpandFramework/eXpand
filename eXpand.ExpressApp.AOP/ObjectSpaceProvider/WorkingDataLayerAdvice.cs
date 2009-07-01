using AopAlliance.Intercept;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public class WorkingDataLayerAdvice : IMethodInterceptor
    {
        private readonly IDataLayer dataLayer;

        public WorkingDataLayerAdvice(IDataLayer dataLayer)
        {
            this.dataLayer = dataLayer;
        }

        public object Invoke(IMethodInvocation invocation)
        {
            return dataLayer;
        }
    }
}