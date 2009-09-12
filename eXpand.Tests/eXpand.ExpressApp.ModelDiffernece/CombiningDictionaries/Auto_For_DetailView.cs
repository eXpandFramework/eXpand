using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class Auto_For_DetailView:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Auto_Combination_OnCurrentObjectChanged_Of_UserDifferenceModel_IsAvaliable_Only_For_DetailView()
        {
            var controller = new CombineUserModelWithActiveUserDifferenceController();
            Assert.AreEqual(typeof(UserModelDifferenceObject), controller.TargetObjectType);
            Assert.AreEqual(ViewType.DetailView, controller.TargetViewType);
        }
        [Test]
        [Isolated]
        public void On_CurrentOBjectChanged_If_DefaultActiveUserDifference_Then_Combine_UserModel_With_it_And_save_it()
        {
            IApplicationUniqueName applicationUniqueName = GetApplicationUniqueName();
            var modelDifferenceObject = new UserModelDifferenceObject(Session.DefaultSession) {PersistentApplication =new PersistentApplication(Session.DefaultSession), Model = new Dictionary(Schema.GetCommonSchema()) };
            var factory = new ViewControllerFactory();
            var controller = factory.CreateController<CombineUserModelWithActiveUserDifferenceController>(ViewType.ListView, modelDifferenceObject);
            var view = controller.View;
            using (RecorderManager.StartRecording()){
                view.CurrentObjectChanged += null;
            }
            factory.Activate(controller);
            var queryUserModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryUserModelDifferenceObject>();
            Isolate.WhenCalled(() => queryUserModelDifferenceObject.GetActiveModelDifference("")).WillReturn((UserModelDifferenceObject)controller.View.CurrentObject);
            var combiner = Isolate.Fake.InstanceAndSwapAll<DictionaryCombiner>();
            Isolate.WhenCalled(() => combiner.AddAspects(modelDifferenceObject)).IgnoreCall();

            ((EventHandler) RecorderManager.LastMockedEvent.GetEventHandle()).Invoke(this,new EventArgs());

            Isolate.Verify.WasCalledWithAnyArguments(() => combiner.AddAspects(modelDifferenceObject));
        }

    }

}