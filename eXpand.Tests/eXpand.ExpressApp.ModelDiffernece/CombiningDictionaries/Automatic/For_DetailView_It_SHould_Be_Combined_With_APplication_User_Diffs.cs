using System;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries.Automatic{
    public class For_DetailView_It_SHould_Be_Combined_With_APplication_User_Diffs:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void When_UserDifferenceObject_Is_Shown()
        {
            var factory = new ViewControllerFactory();
            var activateController = factory.CreateController<CombineActiveUserDifferenceWithUserModelController>(
                typeof(UserModelDifferenceObject));
            bool combined = false;
            Isolate.WhenCalled(() => activateController.CombineWithApplicationUserDiffs()).DoInstead(context => combined = true);

            factory.Activate(activateController);

            Assert.IsTrue(combined);

        }
        [Test]
        [Isolated]
        public void When_Current_UserDifferenceObject_Changed(){
            var factory = new ViewControllerFactory();
            var controller = factory.CreateController<CombineActiveUserDifferenceWithUserModelController>(typeof (UserModelDifferenceObject));
            bool combined = false;
            Isolate.WhenCalled(() => controller.CombineWithApplicationUserDiffs()).DoInstead(context => combined = true);
            factory.Activate(controller,new HandleInfo{CurrentObjectChanged= true});

            factory.CurrentObjectChangedHandler.Invoke(this,EventArgs.Empty);

            Assert.IsTrue(combined);

        }
    }
}