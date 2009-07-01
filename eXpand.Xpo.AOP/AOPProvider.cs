using DevExpress.Xpo.DB;
using eXpand.Xpo.AOP.DataStore;
using Spring.Aop.Framework;
using Spring.Aop.Support;


namespace eXpand.Xpo.AOP
{
    public class AOPProvider
    {
        
        public static IDataStore AddDataStoreAOPs(IDataStore dataStore)
        {
            var factory = new ProxyFactory(dataStore);
            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("SelectData"), new DataStoreAdvice()));
            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("ModifyData"), new DataStoreAdvice()));
            factory.AddIntroduction(new DefaultIntroductionAdvisor(new DataStoreEventsMixin()));
            return (IDataStore)factory.GetProxy();
        }


        
//        public static IConnectionProvider AddBaseDataLayerAdvices(IConnectionProvider connectionProvider)
//        {
//            var factory = new ProxyFactory(connectionProvider);
//            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("ConnectionProvider"),
//                                                     new BaseDatalayerDataStoreAdvice(AddDataStoreAOPs(connectionProvider.ConnectionProvider))));
//            return (IConnectionProvider) factory.GetProxy();
//        }

    }
}