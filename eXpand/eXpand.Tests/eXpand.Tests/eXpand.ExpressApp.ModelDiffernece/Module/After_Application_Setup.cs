using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
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
        
        protected XafApplication application;
        protected ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore> modelDifferenceModule;

        [Test, Isolated]
        public void FireEvent_ParameterWasDefaultValue(){
            int? actualEntryID = null;

            var logger = new RealLogger();
            logger.LogEntryCreated += entryId => actualEntryID = entryId;

            int expectedLogEntryID = 100;
            Isolate.Invoke.Event(() => logger.LogEntryCreated += null, expectedLogEntryID);

            Assert.AreEqual(expectedLogEntryID, actualEntryID);
        } 
        [SetUp]
        public override void Setup(){
            base.Setup();
//            modelDifferenceModule =
//                Isolate.Fake.Instance<ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore>>(Members.CallOriginal);
//            modelDifferenceModule.PersistentApplicationModelUpdated = false;
//            application = Isolate.Fake.Instance<XafApplication>();
//            modelDifferenceModule.Setup(application);

            
//            eventHandler = ((EventHandler<LogonEventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
        }
        [Test]
        [Isolated]
        public void If_PersiststentApp_AlreadyUpdated_Do_Not_Update_Again()
        {
            var instance = Isolate.Fake.Instance<ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore>>(Members.CallOriginal);
            var xafApplication = new WinApplication();

            instance.Setup(xafApplication);

            Isolate.Invoke.Event(() => xafApplication.LoggingOn += null, null, null);
//            Isolate.WhenCalled(() => modelDifferenceModule.UpdatePersistentApplication(null)).WillThrow(new Exception());
////            Isolate.WhenCalled(() => modelDifferenceModule.PersistentApplicationModelUpdated).WillReturn(true);
//            modelDifferenceModule.Setup();
//            Isolate.Invoke.Event(() => application.LoggingOn += null, new LogonEventArgs(null));
//            eventHandler.Invoke(this,new LogonEventArgs(null));
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
            Isolate.WhenCalled(() => modelDifferenceModule.UpdatePersistentApplication(null)).DoInstead(context => {updated=true;return persistentApplication;});

//            eventHandler.Invoke(this,new LogonEventArgs(null));

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

    public class RealLogger{
        public event Func<int, int?> LogEntryCreated;

        private void InvokeLogEntryCreated(int arg){
            Func<int, int?> created = LogEntryCreated;
            if (created != null) created(arg);
        }
    }
}