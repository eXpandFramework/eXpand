using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores
{
    [TestFixture]
    public class Loading_UserModel_Dictionary : eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void If_No_ActiveDifference_Found_Then_A_New_UserDifferenceObject_Should_Be_Saved()
        {
            var application = Isolate.Fake.Instance<XafApplication>();
            application.ApplicationName = "appName";
            var store = new XpoUserModelDictionaryDifferenceStore(Session.DefaultSession, application);
            Isolate.WhenCalled(() => store.GetActiveDifferenceObjects()).WillReturn(new List<ModelDifferenceObject>().AsQueryable());
            Isolate.WhenCalled(() => store.GetActiveDifferenceObjects()).CallOriginal();
            bool saved = false;
            Isolate.WhenCalled(() => store.SaveDifference(null)).DoInstead(context => { saved = true; });


            store.LoadDifference(Schema.GetCommonSchema());

            Assert.IsTrue(saved);
        }
        [Test]
        [Isolated]
        public void When_A_New_UserDifferenceObject_Is_Saved_it_Should_Contain_Same_NUmber_Of_Aspect_As_The_Application(){
            var application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => application.Model.Aspects).WillReturn(new List<string>{DictionaryAttribute.DefaultLanguage,"el"});
            var store = new XpoUserModelDictionaryDifferenceStore(Session.DefaultSession, application);
            Isolate.WhenCalled(() => store.GetActiveDifferenceObjects()).WillReturn(new List<ModelDifferenceObject>().AsQueryable());
            Dictionary dictionary = null;
            Isolate.WhenCalled(() => store.SaveDifference(null)).DoInstead(context => dictionary=(Dictionary) context.Parameters[0]);

            store.LoadDifference(Schema.GetCommonSchema());

            Assert.AreEqual(2, dictionary.Aspects.Count);

        }

        [Test]
        [Isolated]
        public void Is_A_Combination_Of_All_Models_Assign_to_Current_User_For_Current_Aspect_And_Application()
        {
            var store = new XpoUserModelDictionaryDifferenceStore(Session.DefaultSession, Isolate.Fake.Instance<XafApplication>());



            var modelDifferenceObject = new UserModelDifferenceObject(Session.DefaultSession) { Model = DefaultDictionary, PersistentApplication = new PersistentApplication(Session.DefaultSession) { Model = PersistentAppDictionary } };
            Isolate.WhenCalled(() => store.GetActiveDifferenceObjects()).
                WillReturnCollectionValuesOf(new List<ModelDifferenceObject>{
                                                                          modelDifferenceObject,
                                                                          new UserModelDifferenceObject(Session.DefaultSession)
                                                                          {Model = DefaultDictionary2,PersistentApplication = new PersistentApplication(Session.DefaultSession){Model = new Dictionary(Schema.GetCommonSchema())}}
                                                                      }.AsQueryable());

            Dictionary dictionary = store.LoadDifference(Schema.GetCommonSchema());

            var wrapper = new ApplicationNodeWrapper(dictionary).BOModel;
            Assert.IsNotNull(wrapper.FindClassByName("MyClass"));
            Assert.IsNotNull(wrapper.FindClassByName("MyClass2"));
            
        }
    }
}
