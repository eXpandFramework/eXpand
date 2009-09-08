using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Utils.Helpers;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Saving{
    [TestFixture]
    public class PersistentApplications:eXpandBaseFixture{
        [Test]
        [Isolated]
        public void Name_Is_Unique()
        {
            new PersistentApplication(Session.DefaultSession) { Name = "name" }.Save();

            var application = new PersistentApplication(Session.DefaultSession) { Name = "name" };
            var target = new RuleSet().ValidateTarget(application, ContextIdentifier.Save);

            var items =
                target.Results.Where(
                    item =>
                    item.Rule is RuleUniqueValue &&
                    item.Rule.UsedProperties.Contains(application.GetPropertyInfo(x => x.Name).Name)).FirstOrDefault();
            Assert.IsNotNull(items);
            Assert.AreEqual(ValidationState.Invalid, items.State);
        }
//        [Test]
//        [Isolated]
//        public void Schema_Is_Required()
//        {
//            var application = new PersistentApplication(Session.DefaultSession);
//
//            var target = new RuleSet().ValidateTarget(application, ContextIdentifier.Save);
//
//            var resultItem = target.Results.Where(item => item.Rule is RuleRequiredField && item.Rule.UsedProperties.Contains(application.GetPropertyInfo(x => x.Schema).Name)).FirstOrDefault();
//            Assert.IsNotNull(resultItem);
//            Assert.AreEqual(ValidationState.Invalid, resultItem.State);
//        }
        [Test]
        [Isolated]
        public void Schema_Can_Saved()
        {
            var application = new PersistentApplication(Session.DefaultSession) { Model = new Dictionary(Schema.GetCommonSchema()) };
            application.Save();


            var o = (PersistentApplication)new Session(XpoDefault.DataLayer).GetObject(application);

            var value = new DictionaryXmlWriter().GetCurrentAspectXml(Schema.GetCommonSchema().RootNode);

            Assert.AreEqual(value, new DictionaryXmlWriter().GetCurrentAspectXml(o.Model.Schema.RootNode));
        }
        [Test]
        [Isolated]
        public void ApplicationModel_Can_Be_Saved()
        {

            DefaultDictionary.AddAspect("el",elDictionary.RootNode);
            Assert.AreEqual(2, DefaultDictionary.Aspects.Count);
            var application = new PersistentApplication(Session.DefaultSession){Model = DefaultDictionary};

            
            application.Save();

            application.Reload();

            Assert.AreEqual(DefaultDictionary.Schema.RootNode.ToXml(), application.Model.Schema.RootNode.ToXml());
            Assert.AreEqual(DefaultDictionary.RootNode.ToXml(), application.Model.RootNode.ToXml());
        }
    }
}