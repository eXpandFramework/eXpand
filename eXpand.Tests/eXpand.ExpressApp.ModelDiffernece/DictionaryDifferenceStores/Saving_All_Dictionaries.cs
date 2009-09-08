using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores{
    [TestFixture]
    public class Saving_All_Dictionaries:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Create_A_New_Aspect_If_No_Active_Aspect_Found()
        {
            var modelAspectObject = new ModelDifferenceObject(Session.DefaultSession);
            Isolate.Swap.AllInstances<ModelDifferenceObject>().With(modelAspectObject);
            Isolate.WhenCalled(() => modelAspectObject.Save()).IgnoreCall();
            var application = Isolate.Fake.Instance<XafApplication>();
            application.ApplicationName = "ApplicationName";
            var dictionary = new Dictionary(Schema.GetCommonSchema());
            Isolate.WhenCalled(() => dictionary.Aspects).WillReturn(new List<string> { "aspect" });
            Isolate.WhenCalled(() => application.Model).WillReturn(dictionary);
            var store = Isolate.Fake.Instance<XpoDictionaryDifferenceStore>(Members.CallOriginal, ConstructorWillBe.Called, new object[] { Session.DefaultSession, application });
            Isolate.WhenCalled(() => store.GetNewDifferenceObject(Session.DefaultSession)).WillReturn(modelAspectObject);
            Isolate.WhenCalled(() => store.GetActiveDifferenceObject()).WillReturn(null);


            store.SaveDifference(dictionary);

            Isolate.Verify.WasCalledWithAnyArguments(() => modelAspectObject.Save());
        }
        [Test]
        [Isolated]
        public void Update_Values_Of_ActiveAspect_If_Found()
        {
            Isolate.Fake.StaticMethods(typeof(Validator));
            var application = Isolate.Fake.Instance<XafApplication>();
            application.ApplicationName = "ApplicationName";
            var dictionary = new Dictionary();
            Isolate.WhenCalled(() => dictionary.Aspects).WillReturn(new List<string> { "aspect" });
            Isolate.WhenCalled(() => application.Model).WillReturn(dictionary);
            var store = Isolate.Fake.Instance<XpoDictionaryDifferenceStore>(Members.CallOriginal, ConstructorWillBe.Called, new object[] { Session.DefaultSession, application });
            Isolate.Fake.StaticMethods(typeof(ModelDifferenceObjectBuilder));
            var dateCreated = new DateTime();
            var modelStoreObject = new ModelDifferenceObject(Session.DefaultSession){
                                                                                        DateCreated = dateCreated,
                                                                                        PersistentApplication =
                                                                                            new PersistentApplication(Session.DefaultSession){Name = "Name"}
                                                                                    };
            Isolate.WhenCalled(() => store.GetActiveDifferenceObject()).WillReturn(modelStoreObject);

            store.SaveDifference(application.Model);


            Assert.AreEqual("ApplicationName", modelStoreObject.PersistentApplication.Name);
            Assert.AreEqual(dateCreated, modelStoreObject.DateCreated);
        }
        [Test]
        [Isolated]
        public void Validate_ApplicationModel()
        {
            var store = Isolate.Fake.Instance<XpoDictionaryDifferenceStore>(Members.CallOriginal,
                                                                            ConstructorWillBe.Called,
                                                                            new object[]{
                                                                                            Session.DefaultSession,
                                                                                            Isolate.Fake.Instance<XafApplication>()
                                                                                        });
            bool called = false;
            var ruleSet = Validator.RuleSet;
            Isolate.WhenCalled(() => ruleSet.ValidateAll(null, null)).DoInstead(context => called = true);

            store.OnAspectStoreObjectSaving(new ModelDifferenceObject(Session.DefaultSession));

            Assert.IsTrue(called);
        }
        [Test]
        [Isolated]
        public void CauseOf_Application_Is_Unique_It_Should_Check_DataStore_Before_Creating_New()
        {

            var modelDictionaryDifferenceStore = new XpoUserModelDictionaryDifferenceStore(Session.DefaultSession,
                                                                                                                   Isolate.Fake.Instance<XafApplication>());
            new PersistentApplication(Session.DefaultSession) { Name = "appName" }.Save();
            var application = Isolate.Fake.InstanceAndSwapAll<QueryPersistentApplication>();
            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            Isolate.WhenCalled(() => application.Find("")).WillReturn(persistentApplication);
            Isolate.WhenCalled(() => modelDictionaryDifferenceStore.OnAspectStoreObjectSaving(null)).IgnoreCall();
            Isolate.WhenCalled(() => modelDictionaryDifferenceStore.GetActiveDifferenceObject()).WillReturn(null);
            Isolate.WhenCalled(() => modelDictionaryDifferenceStore.GetNewDifferenceObject(Session.DefaultSession)).WillReturn(new ModelDifferenceObject(Session.DefaultSession));

            modelDictionaryDifferenceStore.SaveDifference(new Dictionary(Schema.GetCommonSchema()));

            Isolate.Verify.WasCalledWithAnyArguments(() => application.Find(""));
        }

    }
}