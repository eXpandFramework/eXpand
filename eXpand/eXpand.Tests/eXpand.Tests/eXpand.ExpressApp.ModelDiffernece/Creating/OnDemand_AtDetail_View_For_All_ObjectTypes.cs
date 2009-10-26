using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using eXpand.Xpo;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Creating{
    [TestFixture]
    public class OnDemand_AtDetail_View_For_All_ObjectTypes:XpandBaseFixture
    {

        [Test]
        [Isolated]
        public void Should_InitializeMembers()
        {

            var controller = new ViewControllerFactory().CreateController<CreateNewDifferenceObjectViewController>(ViewType.DetailView,new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)});
            
            var modelAspectObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            bool called = false;
            Isolate.WhenCalled(() => modelAspectObject.InitializeMembers("", "")).DoInstead(context =>
            {
                called = true;
                return modelAspectObject;
            });

            controller.OnObjectCreated(null, new ObjectCreatedEventArgs(modelAspectObject, null));

            Assert.IsTrue(called);
        }
    }
}