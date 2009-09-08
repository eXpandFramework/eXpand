using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Changing
{
    [TestFixture]
    public class ModelDifferenceObjects:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void When_Changing_PrefferedLanguage_Then_Model_CurrentAspect_Should_Be_ModelDifference_CurrentLanguage(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);
            var dictionary = DefaultDictionary;
            modelDifferenceObject.Model=dictionary;

            modelDifferenceObject.PreferredAspect = "el ";

            Assert.AreEqual(modelDifferenceObject.CurrentLanguage, modelDifferenceObject.Model.CurrentAspect);
        }
        [Test]
        [Isolated]
        public void When_Changing_PrefferedLanguage_Then_PersistentApplication_Model_CurrentAspect_Should_Be_ModelDifference_CurrentLanguage(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            var dictionary = DefaultDictionary;
            modelDifferenceObject.Model=DefaultDictionary2;
            modelDifferenceObject.PersistentApplication.Model=dictionary;

            modelDifferenceObject.PreferredAspect = "el ";

            Assert.AreEqual(modelDifferenceObject.CurrentLanguage, modelDifferenceObject.PersistentApplication.Model.CurrentAspect);
        }

        [Test]
        [Isolated]
        public void When_Changing_XmlContent_It_Should_Be_Validated_Against_Application_Schema(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession)
            {
                PersistentApplication =
                    new PersistentApplication(Session.DefaultSession) { Model = new Dictionary(Schema.GetCommonSchema()) },

            };
            var dictionary = Isolate.Fake.InstanceAndSwapAll<Dictionary>(Members.CallOriginal);
            bool validated = false;
            Isolate.WhenCalled(() => dictionary.Validate()).DoInstead(context => validated= true);

            
            modelDifferenceObject.XmlContent = "<Application/>";

            Assert.IsTrue(validated);

        }

        [Test]
        [Isolated]
        public void When_Changing_PreferredAspect_To_Default_Then_XmlContent_Should_Have_Only_The_Default_diff(){

            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { Model = new Dictionary(Schema.GetCommonSchema()) } };
            DefaultDictionary.AddAspect("el", elDictionary.RootNode);
            modelDifferenceObject.Model = DefaultDictionary;

            modelDifferenceObject.PreferredAspect = DictionaryAttribute.DefaultLanguage;

            Assert.AreEqual(new DictionaryXmlReader().ReadFromString(DefaultClassXml).ToXml(), new DictionaryXmlReader().ReadFromString(modelDifferenceObject.XmlContent).ToXml());
        }
        [Test]
        [Isolated]
        public void When_Changing_PrefferedAspect_To_Non_Default_Then_XmlContent_SHould_CurrentAspect_Diffs()
        {
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { Model = new Dictionary(Schema.GetCommonSchema()) } };
            DefaultDictionary.AddAspect("el", elDictionary.RootNode);
            modelDifferenceObject.Model = DefaultDictionary;

            modelDifferenceObject.PreferredAspect = "el";

            Assert.AreEqual(new DictionaryXmlReader().ReadFromString(elClassXml).ToXml(), new DictionaryXmlReader().ReadFromString(modelDifferenceObject.XmlContent).ToXml());

        }
        [Test]
        [Isolated]
        public void When_PrefferedAspect_Because_a_Property_Editor_WillBind_Model_Should_Be_Marked_As_Changed(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            bool marked = false;
            modelDifferenceObject.Changed +=
                (sender, args) => marked = args.PropertyName == modelDifferenceObject.GetPropertyInfo(x => x.Model).Name;

            modelDifferenceObject.PreferredAspect = "el";

            Assert.IsTrue(marked);
        }
    }
}
