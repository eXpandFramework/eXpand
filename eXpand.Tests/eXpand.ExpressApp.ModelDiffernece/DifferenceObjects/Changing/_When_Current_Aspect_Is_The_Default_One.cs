using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Changing{
    public class _When_Current_Aspect_Is_The_Default_One 
    {
        [Test]
        [Isolated]
        public void When_Changing_XmlContent_Then_It_Should_Be_Added_To_Aspects_Collection()
        {
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession)
                                        {
                                            PersistentApplication =
                                                new PersistentApplication(Session.DefaultSession) { Schema = Schema.GetCommonSchema() },
                                            XmlContent = "<Application/>",
                                        };

            Assert.AreEqual(1, modelDifferenceObject.AspectXmls.Count);
            Assert.ContainsKey(modelDifferenceObject.AspectXmls, DictionaryAttribute.DefaultLanguage);
        }
        [Test]
        [Isolated]
        public void When_Changing_Model_Then_Should_Be_Added_Apsects_Collection()
        {
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession)
                                        {
                                            PersistentApplication =
                                                new PersistentApplication(Session.DefaultSession) { Schema = Schema.GetCommonSchema() },
                                            Model = new DictionaryNode("Application"),
                                        };

            Assert.AreEqual(1, modelDifferenceObject.AspectXmls.Count);
            Assert.ContainsKey(modelDifferenceObject.AspectXmls, DictionaryAttribute.DefaultLanguage);
        }


    }
}