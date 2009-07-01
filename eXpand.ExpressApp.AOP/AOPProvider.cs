using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.ExpressApp.AOP.ObjectSpaceProvider;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace eXpand.ExpressApp.AOP
{
    public class AOPProvider
    {
        public static DevExpress.ExpressApp.ObjectSpaceProvider AddObjectSpaceProviderCreateObjectSpaceAdvice(DevExpress.ExpressApp.ObjectSpaceProvider objectSpaceProvider)
        {
            var factory = new ProxyFactory(objectSpaceProvider);
            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("CreateObjectSpace"),
                                                          new CreateObjectSpaceAdvice()));
            factory.AddIntroduction(new DefaultIntroductionAdvisor(new ObjectSpaceProviderObjectSpaceEventsMixin()));
            factory.ProxyTargetType = true;
            return (DevExpress.ExpressApp.ObjectSpaceProvider) factory.GetProxy();
        }

        public static IWorkingDataLayer AddObjectSpaceProviderWorkingDataLayerAdvice(DevExpress.ExpressApp.ObjectSpaceProvider objectSpaceProvider, IDataLayer dataLayer)
        {
            var factory = new ProxyFactory(objectSpaceProvider);
            factory.AddInterface(typeof(IWorkingDataLayer));
            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("get_WorkingDataLayer"),
                                                          new WorkingDataLayerAdvice(dataLayer)));

            return (IWorkingDataLayer) factory.GetProxy();
        }

        public delegate IDataStore DataStoreDelegate(IDataStore dataStore);

        public static DevExpress.ExpressApp.ObjectSpaceProvider AddObjectSpaceProviderCreateWorkingDataLayerAdvice(
            DevExpress.ExpressApp.ObjectSpaceProvider objectSpaceProvider, DataStoreDelegate dataStore)
        {
            var factory = new ProxyFactory(objectSpaceProvider) { ProxyTargetType = true };
            factory.ProxyTargetType = true;
            factory.AddAdvisor(new DefaultPointcutAdvisor(
                                   new SdkRegularExpressionMethodPointcut("CreateWorkingDataLayer"),
                                   new CreateWorkingDataLayerAdvice()));
            factory.AddIntroduction(new DefaultIntroductionAdvisor(new ObjectSpaceProviderDataStoreEventsMixin()));

            var events = ((IObjectSpaceProviderDataStoreEvents)factory.GetProxy());
            
            
            events.WorkingDataLayerCreating += (sender, args) =>
            {
                args.Handled = true;
                args.DataStore = dataStore.Invoke(args.DataStore);
            };
            return (DevExpress.ExpressApp.ObjectSpaceProvider) events;
        }
    }
}
