using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Creating
{
    [TestFixture]
    public class ModelDifferenceObjects:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void DifferenceType_Is_Model()
        {
            var modelAspectObject = new ModelDifferenceObject(Session.DefaultSession);

            Assert.AreEqual(DifferenceType.Model, modelAspectObject.DifferenceType);
        }
        [Test]
        [Isolated]
        public void Cause_PersistentApplication_ShouldBe_Unique_Should_Query_DataStore_Befare_Creating_New(){
            var application = new PersistentApplication(Session.DefaultSession){Name = "appName"};
            application.Save();
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);
            

            modelDifferenceObject.InitializeMembers("appName","");

            Assert.AreEqual(application, modelDifferenceObject.PersistentApplication);
        }
    }
}
