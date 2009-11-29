//using DevExpress.ExpressApp;
//using DevExpress.Xpo;
//using DevExpress.Xpo.DB;
//using DevExpress.Xpo.Helpers;
//using MbUnit.Framework;
//using TypeMock;
//using TypeMock.ArrangeActAssert;
//using XAFPoint.ExpressApp.AOP.ObjectSpaceProvider;
//using XAFPoint.ExpressApp.SkinToDataStore;
//using XAFPoint.Xpo.AOP;
//
//namespace Fixtures.XAFPoint.ExpressApp.SkinToDataStore
//{
//    [TestFixture]
//    public class ObjectSpaceProviderProxyFixture
//    {
//        [Test][VerifyMocks]
//        public void ObjectSpaceCreatedEventWillBeSubsscribedOnSetup()
//        {
//            var providerEvents = Isolate.Fake.Instance<IObjectSpaceProviderObjectSpaceEvents>();
//            using (RecorderManager.StartRecording()) providerEvents.ObjectSpaceCreated += null;
//
//            ObjectSpaceProviderProxy.Setup1(providerEvents);
//        }
//        [Test]
//        [Isolated]
//        public void NewObjectSpaceWillBeCreatedFromExistingProvider()
//        {
//            
//            Isolate.Swap.NextInstance<ConnectionStringDataStoreProvider>().With(Isolate.Fake.Instance<ConnectionStringDataStoreProvider>());
//            var instance = Isolate.Fake.Instance<ObjectSpaceProviderObjectSpaceArgs>();
//            Isolate.WhenCalled(() => instance.ObjectSpaceProvider).WillReturn(
//                Isolate.Fake.Instance<ObjectSpaceProvider>());
//            var dataLayer = Isolate.Fake.Instance<SimpleDataLayer>();
//            Isolate.WhenCalled(() => ((ObjectSpaceProvider) instance.ObjectSpaceProvider).WorkingDataLayer).WillReturn(
//                dataLayer);
//            Isolate.Fake.StaticMethods<AOPProvider>();
//            var dataStore = Isolate.Fake.Instance<IDataStore>();
//            Isolate.WhenCalled(() => AOPProvider.AddDataStoreAOPs(null)).WillReturn(dataStore);
//
//            ObjectSpace objectSpace = ObjectSpaceProviderProxy.CreateObjectSpace(instance);
//
//            Assert.AreEqual(dataStore, ((BaseDataLayer) objectSpace.Session.DataLayer).ConnectionProvider);
//        }
//    }
//}
