using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores{
    [TestFixture]
    public class Getting_Difference_Objects_From_Application_Dictionary:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void A_New_Instance_Of_ModelDifferenceObject_Should_Be_Returned_As_New_Store()
        {
            Mock<ModelDifferenceObject> mock = MockManager.MockAll<ModelDifferenceObject>();

            TrackInstances<ModelDifferenceObject> instances = mock.TrackInstances();

            var store = Isolate.Fake.Instance<XpoModelDictionaryDifferenceStore>(Members.CallOriginal, ConstructorWillBe.Called,
                                                                         Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), false);

            ModelDifferenceObject modelDifferenceObject = store.GetNewDifferenceObject(Session.DefaultSession);

            Assert.AreEqual(modelDifferenceObject, instances.LastInstance);
        }
        [Test]
        [Isolated]
        public void As_Active_Difference_Will_Return_Active_ModelActiveObjects()
        {
            var store = Isolate.Fake.Instance<XpoModelDictionaryDifferenceStore>(Members.CallOriginal, ConstructorWillBe.Called,
                                                                         Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), false);
            Isolate.Fake.StaticMethods(typeof(ModelDifferenceObjectBuilder));
            var modelStoreObject = new ModelDifferenceObject(Session.DefaultSession);
            var queryModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryModelDifferenceObject>();
            Isolate.WhenCalled(() => queryModelDifferenceObject.GetActiveModelDifference("")).WillReturn(modelStoreObject);

            ModelDifferenceObject newDifference = store.GetActiveDifferenceObject();

            Assert.AreEqual(modelStoreObject, newDifference);
        }

    }
}