using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class Automatic_For_DetailView:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Auto_Combination_OnCurrentObjectChanged_Of_UserDifferenceModel_IsAvaliable_Only_For_DetailView()
        {
            var controller = new CombineActiveUserDifferenceWithUserModelController();
            Assert.AreEqual(typeof(UserModelDifferenceObject), controller.TargetObjectType);
            Assert.AreEqual(ViewType.DetailView, controller.TargetViewType);
        }
        public class It_SHould_Be_Combined_With_APplication_User_Diffs : eXpandBaseFixture
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
            public void When_Current_UserDifferenceObject_Changed()
            {
                var factory = new ViewControllerFactory();
                var controller = factory.CreateController<CombineActiveUserDifferenceWithUserModelController>(typeof(UserModelDifferenceObject));
                bool combined = false;
                Isolate.WhenCalled(() => controller.CombineWithApplicationUserDiffs()).DoInstead(context => combined = true);
                factory.Activate(controller, new HandleInfo { CurrentObjectChanged = true });

                factory.CurrentObjectChangedHandler.Invoke(this, EventArgs.Empty);

                Assert.IsTrue(combined);

            }
        }

        [Test]
        [Isolated]
        public void If_CurrentUserDIfferenceObject_Is_Active_One_Then_Combine_It_With_ApplicationUserDiffs(){
            
            var queryUserModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryUserModelDifferenceObject>();

            var controller = new ViewControllerFactory().CreateController<CombineActiveUserDifferenceWithUserModelController>(ViewType.DetailView, new UserModelDifferenceObject(Session.DefaultSession));
            var userModelDifferenceObject = ((UserModelDifferenceObject) controller.View.CurrentObject);
            Dictionary dictionary = DefaultDictionary.Clone();
            Isolate.WhenCalled(() => userModelDifferenceObject.GetCombinedModel()).WillReturn(dictionary);
            Dictionary model = controller.Application.Model;
            Isolate.WhenCalled(() => model.GetDiffs()).WillReturn(DefaultDictionary2.Clone());
            Isolate.WhenCalled(() => queryUserModelDifferenceObject.GetActiveModelDifference("")).WillReturn((UserModelDifferenceObject) controller.View.CurrentObject);

            controller.CombineWithApplicationUserDiffs();

            DefaultDictionary.CombineWith(DefaultDictionary2);
            Assert.AreEqual(DefaultDictionary2.RootNode.ToXml(), userModelDifferenceObject.Model.RootNode.ToXml());
        }


    }
}