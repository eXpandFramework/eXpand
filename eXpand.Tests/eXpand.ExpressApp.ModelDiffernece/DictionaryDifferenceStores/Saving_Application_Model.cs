using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores{
    [TestFixture]
    public class Saving_Application_Model:eXpandBaseFixture{
        [Test]
        [Isolated]
        public void When_Saving_Should_Combine_With_Diffs(){
            var modelDictionaryDifferenceStore = Isolate.Fake.Instance<XpoModelDictionaryDifferenceStore>(Members.CallOriginal);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);

            modelDictionaryDifferenceStore.OnAspectStoreObjectSaving(modelDifferenceObject, DefaultDictionary);

            Assert.AreEqual(DefaultDictionary.RootNode.ToXml(), modelDifferenceObject.Model.RootNode.ToXml());
        }
    }
}