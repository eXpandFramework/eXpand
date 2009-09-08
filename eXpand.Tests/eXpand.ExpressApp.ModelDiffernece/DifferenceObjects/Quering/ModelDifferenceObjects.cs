using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Quering{
    [TestFixture(Order = 1)]
    public class ModelDifferenceObjects:eXpandBaseFixture
    {
        
        [Test]
        public void As_ActiveDifferenceObjectss_For_AnApplication_should_return_With_The_Same_Application(){
            var defaultModelDifferenceObjectsObject = new ModelDifferenceObject(Session.DefaultSession)
                                                      {PersistentApplication =new PersistentApplication(Session.DefaultSession){Name = "appName"}};
            defaultModelDifferenceObjectsObject.Save();
            var elModelDifferenceObjectsObject = new ModelDifferenceObject(Session.DefaultSession)
                                                 {PersistentApplication =new PersistentApplication(Session.DefaultSession){Name = "appName"}};
            elModelDifferenceObjectsObject.Save();
            var modelDifferenceObject = new UserModelDifferenceObject(Session.DefaultSession)
                                        {PersistentApplication =new PersistentApplication(Session.DefaultSession){Name = "appName"}};
            modelDifferenceObject.Save();
            
            
            IQueryable<ModelDifferenceObject> modelDifferenceObjectsObjects = new QueryModelDifferenceObject(Session.DefaultSession).GetActiveModelDifferences("appName");

            Assert.AreEqual(3, modelDifferenceObjectsObjects.Count());
        }
        [Test]
        [Isolated]
        public void Disabled_DifferenceObjectss_Should_Not_Be_Returned_As_ActiveDifferenceObjectss()
        {
            new ModelDifferenceObject(Session.DefaultSession){
                                                                 PersistentApplication =new PersistentApplication(Session.DefaultSession){Name = "AppName"},
                                                                 Disabled = false
                                                             }.Save();
            new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession){Name = "AppName"},  Disabled = true}.Save();

            IQueryable<ModelDifferenceObject> stores =
                new QueryModelDifferenceObject(Session.DefaultSession).GetActiveModelDifferences("AppName");

            Assert.AreEqual(1, stores.Count());

        }
        [Test]
        [Isolated]
        public void ActiveDifferenceObject_Is_Of_Model_Differnce()
        {
            new UserModelDifferenceObject(Session.DefaultSession)
            {
                PersistentApplication = new PersistentApplication(Session.DefaultSession){ Name = "AppName" },
            }.Save();

            var difference = new QueryModelDifferenceObject(Session.DefaultSession).GetActiveModelDifference("AppName");

            Assert.IsNull(difference);
        }
    }
}