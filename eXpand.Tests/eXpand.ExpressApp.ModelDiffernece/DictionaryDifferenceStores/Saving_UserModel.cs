using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.Security;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores
{
    [TestFixture]
    public class Saving_UserModel:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        [Row(true)]
        [Row(false, ExpectedException = typeof(NotImplementedException))]
        public void if_AspectObject_Is_Not_Persistent_WillNot_be_saved(bool nonPersistent)
        {
            var store = new XpoUserModelDictionaryDifferenceStore(Isolate.Fake.Instance<XafApplication>());

            var modelStoreObject = new UserModelDifferenceObject(Session.DefaultSession){
                                                                                            PersistentApplication = new PersistentApplication(Session.DefaultSession),
                                                                                            NonPersistent = nonPersistent
                                                                                        };
            Isolate.WhenCalled(() => modelStoreObject.Save()).WillThrow(new NotImplementedException());
            Isolate.WhenCalled(() => store.GetActiveDifferenceObject()).WillReturn(modelStoreObject);
            var dictionary = new Dictionary(Schema.GetCommonSchema());
            Isolate.WhenCalled(() => dictionary.Aspects).WillReturn(new List<string> { "aspect" });
            Isolate.Fake.StaticMethods(typeof(Validator));

            store.SaveDifference(dictionary);
        }
        [Test]
        [Isolated]
        public void If_User_Has_Permnission_To_Combine_With_Application_Model_Combination_Should_Occur()
        {
            ModelCombinePermission permission = null;
            Isolate.WhenCalled(() => SecuritySystem.IsGranted(null)).DoInstead(context =>
            {
                permission = context.Parameters[0] as ModelCombinePermission;
                return true;
            });
            var modelAspectObject = new ModelDifferenceObject(Session.DefaultSession){Model =DefaultDictionary,PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            var queryModelAspectObject = Isolate.Fake.InstanceAndSwapAll<QueryModelDifferenceObject>();
            Isolate.WhenCalled(() => queryModelAspectObject.GetActiveModelDifference(  "")).WillReturn(modelAspectObject);
            var store = new XpoUserModelDictionaryDifferenceStore( Isolate.Fake.Instance<XafApplication>());
            var aspectObject = new UserModelDifferenceObject(Session.DefaultSession){
                                                                                        PersistentApplication = new PersistentApplication(Session.DefaultSession),
                                                                                        NonPersistent = true,
                                                                                        Model = DefaultDictionary2
                                                                                    };
            
            store.OnDifferenceObjectSaving(aspectObject, new Dictionary());


            Assert.IsNotNull(permission);
            Assert.AreEqual(ApplicationModelCombineModifier.Allow, permission.Modifier);
            Assert.IsFalse(modelAspectObject.IsNewObject);
            Assert.IsNotNull(new ApplicationNodeWrapper(modelAspectObject.Model).BOModel.FindClassByName("MyClass2"));
        }


        [Test]
        [Isolated]
        public void When_Saving_It_Should_Combined_With_Application_Diffs(){
            Isolate.WhenCalled(() => Validator.RuleSet.ValidateAll(null, null)).ReturnRecursiveFake();
            var modelDictionaryDifferenceStore = new XpoUserModelDictionaryDifferenceStore( Isolate.Fake.Instance<XafApplication>());
            var modelDifferenceObject = new UserModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession),Model = DefaultDictionary};

            modelDictionaryDifferenceStore.OnDifferenceObjectSaving(modelDifferenceObject, elDictionary);

            Assert.AreEqual("el", new ApplicationNodeWrapper(modelDifferenceObject.Model).BOModel.FindClassByName("MyClass").Caption);
        }
    }
}
