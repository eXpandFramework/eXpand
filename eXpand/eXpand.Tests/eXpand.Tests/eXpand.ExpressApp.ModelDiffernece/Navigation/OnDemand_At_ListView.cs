using System;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Security.Controllers;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Navigation{
    /// <summary>
    /// Navigation_To_ModelAspect_ListView_Requires_Permission
    /// </summary>
    [TestFixture]
    public class OnDemand_At_ListView
    {
        [Test]
        [Isolated]
        [Row(typeof(UserModelDifferenceObject))]
        [Row(typeof(ModelDifferenceObject), ExpectedException = typeof(SecurityException))]
        public void Requires_Permission(Type objectType){
            var application = Isolate.Fake.Instance<XafApplication>();
            var controller = new NavigationItemsController{Application = application};
            var args = new CustomShowNavigationItemEventArgs(null);
            Isolate.WhenCalled(() => args.FitToObjectType(application, objectType)).WithExactArguments().WillReturn(true);
            Isolate.WhenCalled(() => args.FitToObjectType(application, objectType)).WillReturn(false);
            Isolate.WhenCalled(() => SecuritySystem.IsGranted(null)).WillReturn(false);

            

            controller.ControllerOnCustomShowNavigationItem(null,args);


        }
    }
}