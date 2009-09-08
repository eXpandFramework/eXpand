using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class DictionaryCombiner_Tests:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Can_Combine_DictionaryNodes_Together(){
            var dictionary = new DictionaryNode("Application");
            var combiner = new DictionaryCombiner(dictionary);
            var dictionaryNode = new DictionaryNode("Application");
            dictionaryNode.AddChildNode("node");

            combiner.CombineWith(dictionaryNode);

            Assert.IsNotNull(dictionary.FindChildNode("node"));
        }
        [Test]
        [Isolated]
        public void Can_Combine_Dictionaries_Together()
        {
            var dictionary = new Dictionary(new DictionaryNode("Application"));
            var combiner = new DictionaryCombiner(dictionary);
            var dictionaryNode = new DictionaryNode("Application");
            dictionaryNode.AddChildNode("node");

            combiner.CombineWith(new Dictionary(dictionaryNode));

            Assert.IsNotNull(dictionary.RootNode.FindChildNode("node"));
        }

        [Test]
        [Isolated]
        public void Can_Combiner_ModelDifferenceObjects_Together(){

            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){Model = DefaultDictionary,PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            var combiner = new DictionaryCombiner(modelDifferenceObject);

            combiner.CombineWith(new ModelDifferenceObject(Session.DefaultSession) { Model = DefaultDictionary2,PersistentApplication = new PersistentApplication(Session.DefaultSession)});

            Assert.IsNotNull(new ApplicationNodeWrapper(modelDifferenceObject.Model).BOModel.FindClassByName("MyClass2"));
        }
        [Test]
        [Isolated]
        public void Can_Combine_DictionaryNode_And_ModelDifference_Together(){
//            var dictionary = new DictionaryNode("Application");
            var combiner = new DictionaryCombiner(DefaultDictionary);
            
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){Model = DefaultDictionary2,PersistentApplication = new PersistentApplication(Session.DefaultSession)};

            combiner.CombineWith(modelDifferenceObject);

            Assert.IsNotNull(new ApplicationNodeWrapper(DefaultDictionary).BOModel.FindClassByName("MyClass2"));
        }
    }
}