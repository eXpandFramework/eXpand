using System.Linq;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Utils.Helpers;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Saving_PersistentApplications : eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Name_Is_Unique()
        {
            new PersistentApplication(Session.DefaultSession) { UniqueName = "name" }.Save();

            var application = new PersistentApplication(Session.DefaultSession) { Name = "name" };
            var target = new RuleSet().ValidateTarget(application, ContextIdentifier.Save);

            var items =
                target.Results.Where(
                    item =>
                    item.Rule is RuleUniqueValue &&
                    item.Rule.UsedProperties.Contains(application.GetPropertyInfo(x => x.UniqueName).Name)).FirstOrDefault();
            Assert.IsNotNull(items);
            Assert.AreEqual(ValidationState.Invalid, items.State);
        }

    }
}