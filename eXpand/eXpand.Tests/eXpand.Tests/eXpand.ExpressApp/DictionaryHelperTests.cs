using System.Collections.Generic;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp
{
    [TestFixture]
    public class DictionaryHelperTests
    {
        [Test]
        [Isolated]
        public void Test()
        {
            const string elXml = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"el&amp;caption\"></Class></BOModel></Application>";
            const string xml = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"capt&quot;ion\"></Class></BOModel></Application>";
            var elDict = new Dictionary(new DictionaryXmlReader().ReadFromString(elXml), Schema.GetCommonSchema());
            var dict = new Dictionary(new DictionaryXmlReader().ReadFromString(xml), Schema.GetCommonSchema());
            Dictionary dictionary = dict.Clone();
            dictionary.AddAspect("el", elDict.RootNode);

            var dictionaryHelper = new DictionaryHelper();
            string aspectFromXml = dictionaryHelper.GetAspectFromXml(new List<string> { "el" }, dictionary.RootNode);

            Assert.AreEqual(dict.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(aspectFromXml).ToXml());
//            Assert.AreEqual(elDict.RootNode.ToXml(), new DictionaryXmlReader().ReadFromString(dictionaryHelper.AspectValues["el"]).ToXml());


        }
    }
}
