using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Deleting_ModelDifferenceObjects:XpandBaseFixture
    {
        [Test][Isolated]
        public void Can_Be_Deleted(){
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            modelDifferenceObject.Save();

            modelDifferenceObject.Delete();

            Assert.IsTrue(modelDifferenceObject.IsDeleted);
        }
    }
}