using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Module{
    [TestFixture]
    public class After_Application_Setup:eXpandBaseFixture{
        protected EventHandler<LogonEventArgs> eventHandler;
        protected XafApplication application;
        protected ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore> modelDifferenceModule;

        [SetUp]
        public override void Setup(){
            base.Setup();
            modelDifferenceModule =
                Isolate.Fake.Instance<ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore>>(Members.CallOriginal);
            application = Isolate.Fake.Instance<XafApplication>();
                
            using (RecorderManager.StartRecording()){
                application.LoggingOn += null;
            }
            modelDifferenceModule.Setup(application);
            eventHandler = ((EventHandler<LogonEventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
        }
        [Test]
        [Isolated]
        public void If_PersiststentApp_AlreadyUpdated_Do_Not_Update_Again()
        {
            Isolate.WhenCalled(() => modelDifferenceModule.UpdatePersistentApplication(null, null)).WillThrow(new Exception());
            Isolate.WhenCalled(() => modelDifferenceModule.PersistentApplicationModelUpdated).WillReturn(true);

            eventHandler.Invoke(this,new LogonEventArgs(null));
        }
        [Test]
        [Isolated]
        public void If_PersistentApp_Not_Updated_Update_It_MarkIt_And_Save_It()
        {
            bool updated = false;
            var objectSpace = Isolate.Fake.Instance<ObjectSpace>();
            Isolate.WhenCalled(() => objectSpace.Session).WillReturn(Session.DefaultSession);
            Isolate.WhenCalled(() => ObjectSpace.FindObjectSpace(null)).WillReturn(objectSpace);
            bool commited = false;
            Isolate.WhenCalled(() => objectSpace.CommitChanges()).DoInstead(context => commited= true);
            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            Isolate.WhenCalled(() => modelDifferenceModule.GetPersistentApplication(null)).WillReturn(persistentApplication);
            Isolate.WhenCalled(() => modelDifferenceModule.UpdatePersistentApplication(null, null)).DoInstead(context => {updated=true;return persistentApplication;});

            eventHandler.Invoke(this,new LogonEventArgs(null));

            Assert.IsTrue(updated);
            Assert.IsTrue((bool) modelDifferenceModule.PersistentApplicationModelUpdated);
            Assert.IsTrue(commited);

        }
        [Test]
        [Isolated]
        public void Search_If_Persistent_App_Exists()
        {
            var queryPersistentApplication = Isolate.Fake.InstanceAndSwapAll<QueryPersistentApplication>();
                
            PersistentApplication persistentApplication = modelDifferenceModule.GetPersistentApplication(application);

            Assert.AreEqual(queryPersistentApplication.Find(""), persistentApplication);
        }
    }
}