using AopAlliance.Intercept;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.AOP.DataLayer
{
    public class BaseDatalayerDataStoreAdvice : IMethodInterceptor
    {
        private readonly IDataStore dataStore;

        public BaseDatalayerDataStoreAdvice(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }
        #region IMethodInterceptor Members
        public object Invoke(IMethodInvocation invocation)
        {
            return dataStore;
        }
        #endregion
    }
}