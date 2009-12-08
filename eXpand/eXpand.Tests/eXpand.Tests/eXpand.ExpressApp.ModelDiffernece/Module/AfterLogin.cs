using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Module{
    [TestFixture(Order = 2)]
    public class AfterLogin:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void UserModel_Should_Be_Assign()
        {
            var module = new ModelDifferenceModule();
            var application = Isolate.Fake.Instance<XafApplication>();
            using (RecorderManager.StartRecording())
            {
                application.CreateCustomUserModelDifferenceStore += null;
            }
            module.Setup(application);
            var handler = ((EventHandler<CreateCustomModelDifferenceStoreEventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
            var args = new CreateCustomModelDifferenceStoreEventArgs();

            handler.Invoke(this, args);

            Assert.IsTrue(args.Handled);
            Assert.IsInstanceOfType(typeof(XpoUserModelDictionaryDifferenceStore), args.Store);
        }
//        [Test]
//        [Isolated]
//        public void After_LoggedIn_All_User_Models_Will_Persist_To_The_DataStore()
//        {
//            var module = new ModelDifferenceModule();
//            var application = Isolate.Fake.Instance<XafApplication>();
//            bool saved = false;
//            Isolate.WhenCalled(() => application.Model.LastDiffStore.SaveDifference(null)).DoInstead(context => saved = true);
//            using (RecorderManager.StartRecording())
//            {
//                application.LoggedOn += null;
//            }
//            module.Setup(application);
//            var handler = ((EventHandler<LogonEventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
//
//            handler.Invoke(this, new LogonEventArgs(null));
//
//            Assert.IsTrue(saved);
//        }

    }
}