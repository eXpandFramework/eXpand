using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class ViewsFactoryTests:eXpandBaseFixture{
        [Test]
        [Isolated]
        public void Can_Create_ListView_Controller_For_A_Type()
        {
            var factory = new ControllerFactory();

            var viewController1 = factory.CreateController<ViewController>(typeof(ModelDifferenceObject));

            Assert.IsNotNull(viewController1.View);
            Assert.IsInstanceOfType(typeof(ListView), viewController1.View);
        }
        [Test]
        [Isolated]
        public void Can_Create_ListView_For_An_Object(){
            var factory = new ControllerFactory();
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};

            var controller = factory.CreateController<ViewController>(modelDifferenceObject);

            Assert.IsNotNull(controller.View);
        }
        [Test]
        [Isolated]
        public void When_Creating_A_ListView_For_An_Object_Current_Object_Should_Be_Set(){
            var factory = new ControllerFactory();
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};

            var controller = factory.CreateController<ViewController>(modelDifferenceObject);

            Assert.AreEqual(modelDifferenceObject.Oid, ((ModelDifferenceObject) controller.View.CurrentObject).Oid);
        }
    }
}