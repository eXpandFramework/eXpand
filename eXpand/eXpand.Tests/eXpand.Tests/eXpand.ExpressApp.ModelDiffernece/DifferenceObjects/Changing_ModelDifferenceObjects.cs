using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using System.Linq;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Changing_ModelDifferenceObjects:XpandBaseFixture
    {
        [Test]
        public void Test()
        {
            var ruleSet = new RuleSet();
            Session.DefaultSession.GetClassInfo(typeof(User)).CreateMember("Test",typeof(string)).AddAttribute(new RuleRequiredFieldAttribute(null, DefaultContexts.Save));

            var target = ruleSet.ValidateTarget(new User(Session.DefaultSession), ContextIdentifier.Save);

            
            RuleSetValidationResultItem ruleRequiredField = target.Results.Where(item => item.Rule is RuleRequiredField&&item.Rule.UsedProperties.Contains("FirstName")).Single();
            Assert.AreEqual(ValidationState.Invalid, ruleRequiredField.State);
        }

        [Test]
        [Isolated]
        public void When_Changing_PrefferedLanguage_Then_Model_CurrentAspect_Should_Be_ModelDifference_CurrentLanguage(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
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
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){
                                                                                             PersistentApplication =new PersistentApplication(Session.DefaultSession) { Model = DefaultDictionary }
                                                                                         };
            var dictionary = Isolate.Fake.InstanceAndSwapAll<Dictionary>(Members.CallOriginal,ConstructorWillBe.Called);
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
        public void When_PrefferedAspect_Change_Then_Its_Aspect_Should_Be_Added_To_ALl_Models(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            Dictionary dictionary = DefaultDictionary;
            modelDifferenceObject.Model=dictionary;
            modelDifferenceObject.PersistentApplication.Model=DefaultDictionary2;

            modelDifferenceObject.PreferredAspect = "el blah";

            Assert.AreEqual("el", modelDifferenceObject.CurrentLanguage);
            Assert.Contains(modelDifferenceObject.Model.Aspects,"el");
            Assert.Contains(modelDifferenceObject.PersistentApplication.Model.Aspects,"el");
        }
    }
}