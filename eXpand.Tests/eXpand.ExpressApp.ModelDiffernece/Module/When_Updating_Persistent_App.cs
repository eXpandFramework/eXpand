using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Module{
    [TestFixture]
    public class When_Updating_Persistent_App : eXpandBaseFixture
    {
        protected EventHandler<EventArgs> eventHandler;
        protected XafApplication application;
        protected ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore> modelDifferenceModule;
        [SetUp]
        public override void Setup(){
            base.Setup();
            modelDifferenceModule =
                                    Isolate.Fake.Instance<ModelDifferenceBaseModule<XpoModelDictionaryDifferenceStore>>();
            application = Isolate.Fake.Instance<XafApplication>();
            Session session = Session.DefaultSession;
            
            using (RecorderManager.StartRecording()){
                application.SetupComplete += null;
            }
            modelDifferenceModule.Setup(application);
            eventHandler = ((EventHandler<EventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
            var objectSpace = Isolate.Fake.Instance<ObjectSpace>();
            Isolate.WhenCalled(() => objectSpace.Session).WillReturn(session);
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(objectSpace);
        }
        [Test]
        [Isolated]
        public void If_Persistent_App_Not_Exist_Create_New(){
            PersistentApplication persistentApplication = modelDifferenceModule.UpdatePersistentApplication(application);
            Assert.IsNotNull(persistentApplication);
        }
        [Test]
        [Isolated]
        public void Set_UniqueName_To_Application_Type_FullName(){
            var persistentApplication = new PersistentApplication(Session.DefaultSession);

            modelDifferenceModule.UpdatePersistentApplication(application);

            Assert.AreEqual(application.GetType().FullName, persistentApplication.UniqueName);
        }
        [Test]
        [Isolated]
        public void Set_Model_To_Application_Model()
        {
            var persistentApplication = new PersistentApplication(Session.DefaultSession);

            modelDifferenceModule.UpdatePersistentApplication(application);

            Assert.AreEqual(application.Model, persistentApplication.Model);
        }
        [Test]
        [Isolated]
        public void Set_Name_To_Application_Title()
        {
            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            application.Title = "title";

            modelDifferenceModule.UpdatePersistentApplication(application);

            Assert.AreEqual("title", persistentApplication.Name);
        }
        [Test]
        [Isolated]
        public void If_Name_Is_Set_Do_Not_Update_Ite()
        {
            var persistentApplication = new PersistentApplication(Session.DefaultSession){Name = "name"};

            application.Title = "title";

            modelDifferenceModule.UpdatePersistentApplication(application);

            Assert.AreEqual("name", persistentApplication.Name);
        }
    }
}