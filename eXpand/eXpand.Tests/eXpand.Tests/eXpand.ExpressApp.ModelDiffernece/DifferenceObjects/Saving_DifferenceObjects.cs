using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using eXpand.Utils.Helpers;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Saving_DifferenceObjects:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Model_With_UpDownDotSymbol_Inside_AttributeValue_Is_Valid()
        {
            string ss = "{0:ProductName}<br>{0:Version}<br><br>{0:Copyright}<br><br>{0:Company}<br><br>{0:Description}".XMLEncode();
            string formatStr = "<Application><Views><DetailView><Items><StaticText ID=\"AboutText\" Text=\"" +ss+ "\"/></Items></DetailView></Views></Application>";
            DictionaryNode readFromString = new DictionaryXmlReader().ReadFromString(formatStr);
            var dictionary1 = new Dictionary(readFromString, Schema.GetCommonSchema());
            var differenceObject = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            differenceObject.Model=dictionary1.Clone();
            differenceObject.Save();

            differenceObject.Reload();

            Assert.AreEqual(dictionary1.RootNode.ToXml(), differenceObject.Model.RootNode.ToXml());
            
        }

        [Test]
        [Isolated]
        public void Model_With_Space_B4_Character_Of_End_Tag_Can_Be_Saved()
        {
            const string s = "<Application><Validation ><ErrorMessageTemplates ><RuleBase MessageTemplateCollectionValidationMessageSuffix=\"1(For the &quot;{TargetCollectionOwnerType}.{TargetCollectionPropertyName}&quot; collection elements).\"/></ErrorMessageTemplates></Validation></Application>";
            Dictionary dictionary = DefaultDictionary.Clone();
            dictionary.AddAspect("de", new DictionaryXmlReader().ReadFromString(s));
            var differenceObject = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            differenceObject.Model = dictionary;
            differenceObject.Save();

            differenceObject.Reload();

            Assert.AreEqual(new DictionaryXmlWriter().GetCurrentAspectXml(DefaultDictionary.RootNode), new DictionaryXmlWriter().GetCurrentAspectXml(differenceObject.Model.RootNode));

            
        }

        [Test]
        [Isolated]
        public void ApplicationModel_with_one_aspect_Can_Be_Saved()
        {
            var dictionary1 = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"Default\"></Class><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"),Schema.GetCommonSchema());
            var dictionary =dictionary1.Clone();
            var elDict = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"elDefault\" AttributeWithOnlyAspect=\"something\"></Class><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            dictionary.AddAspect("el", elDict.RootNode);
            var application =Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called,Session.DefaultSession);
            application.Model = dictionary;
            application.Save();

            application.Reload();

            Assert.AreEqual(dictionary1.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(new DictionaryXmlWriter().GetAspectXML(DictionaryAttribute.DefaultLanguage, application.Model.RootNode)).ToXml());
            Assert.AreEqual(elDict.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(new DictionaryXmlWriter().GetAspectXML("el", application.Model.RootNode)).ToXml());
        }
        [Test]
        [Isolated]
        public void ApplicationModel_with_more_than_one_aspect_Can_Be_Saved()
        {
            var dictionary1 = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"Default\"></Class><Class Name=\"MyClass2\" Caption=\"Default2\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            var dictionary = dictionary1.Clone();
            var elDict = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"elDefault\" AttributeWithOnlyAspect=\"elsomething\"></Class><Class Name=\"MyClass2\" Caption=\"Default2\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            dictionary.AddAspect("el", elDict.RootNode);
            var deDict = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"deDefault\" DeAttributeWithOnlyAspect=\"desomething\"></Class><Class Name=\"MyClass2\" Caption=\"Default2\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            dictionary.AddAspect("de", deDict.RootNode);
            var application = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            application.Model=dictionary;
            application.Save();

            application.Reload();

            var dictionaryXmlWriter = new DictionaryXmlWriter();
            Assert.AreEqual(dictionary1.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(dictionaryXmlWriter.GetAspectXML(DictionaryAttribute.DefaultLanguage, application.Model.RootNode)).ToXml());
            Assert.AreEqual(elDict.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(dictionaryXmlWriter.GetAspectXML("el", application.Model.RootNode)).ToXml());
        }

        [Test]
        [Isolated]
        public void Application_Model_With_More_Than_one_Aspect_And_Spaces_Between_Attribute_Values_Can_Be_Saved(){
            const string defaultS = "ProtectedContentText=\"Protected Content\" PreferredLanguage=\"el\" VersionFormat=\"Version {0}.{1}.{2}\" Title=\"Solution1\" Logo=\"ExpressAppLogo\" Company=\"Endligh\" WebSite=\"1\" CanClose=\"True\" ";
            var dictionary1 = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application "+defaultS + "><BOModel><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            Dictionary dictionary = dictionary1.Clone();
            dictionary1.AddAspect("el", new DictionaryXmlReader().ReadFromString("<Application Company=\"Greek\"></Application>"));
            var application = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            application.Model=dictionary1;
            application.Save();

            application.Reload();


            var dictionaryXmlWriter = new DictionaryXmlWriter();
            Assert.AreEqual(dictionary.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(dictionaryXmlWriter.GetAspectXML(DictionaryAttribute.DefaultLanguage, application.Model.RootNode)).ToXml());
        }

        [Test]
        [Isolated]
        public void ApplicationModel_That_Has_As_First_Aspect_Non_Default_And_Has_More_Than_One_Can_Be_Saved()
        {
            var dictionary1 = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" ></Class><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            var dictionary = dictionary1.Clone();
            var elDict = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"elDefault\" AttributeWithOnlyAspect=\"something\"></Class><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            dictionary.AddAspect("el", elDict.RootNode);
            var defDict = new Dictionary(new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"Default\" ></Class><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>"), Schema.GetCommonSchema());
            dictionary.AddAspect(DictionaryAttribute.DefaultLanguage, defDict.RootNode);
            var application = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            application.Model=dictionary;
            application.Save();

            application.Reload();

            dictionary1.AddAspect(DictionaryAttribute.DefaultLanguage,defDict.RootNode);
            Assert.AreEqual(dictionary1.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(new DictionaryXmlWriter().GetAspectXML(DictionaryAttribute.DefaultLanguage, application.Model.RootNode)).ToXml());

        }

        [Test]
        [Isolated]
        public void ApplicationModel_Can_Be_Saved_When_An_Aspect_Is_Empty()
        {
            DefaultDictionary.AddAspect("el", new DictionaryNode("Application"));
            var application = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal, ConstructorWillBe.Called, Session.DefaultSession);
            application.Model=DefaultDictionary;
            application.Save();

            application.Reload();

            Assert.AreEqual(DefaultDictionary.Schema.RootNode.ToXml(), application.Model.Schema.RootNode.ToXml());
            Assert.AreEqual(DefaultDictionary.RootNode.ToXml(), application.Model.RootNode.ToXml());
        }

        [Test]
        [Isolated]
        public void Model_With_Only_Def_Aspect_Can_Be_Saved(){
            var differenceObject = Isolate.Fake.Instance<DifferenceObject>(Members.CallOriginal,ConstructorWillBe.Called,Session.DefaultSession);
            differenceObject.Model = DefaultDictionary;
            differenceObject.Save();

            differenceObject.Reload();

            Assert.AreEqual(DefaultDictionary.RootNode.ToXml(), differenceObject.Model.RootNode.ToXml());
        }
    }
}