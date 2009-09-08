using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Quering{
    [TestFixture]
    public class PersistentApplications:eXpandBaseFixture
    {
        [Test]
        public void Can_Be_Searched_By_Name(){
            var persistentApplication1 = new PersistentApplication(Session.DefaultSession){Name = "appname"};
            persistentApplication1.Save();
            var application = new QueryPersistentApplication(Session.DefaultSession);

            var persistentApplication = application.Find("appname");

            Assert.AreEqual(persistentApplication1, persistentApplication);
        }
    }
}