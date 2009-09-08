using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class OnDemand_At_Difference_DetailView : OnDemand_At_All_Views<CombineDifferencesOnDemandDetailViewController>{
        [Test]
        [Isolated]
        public void Create_Popup_ListView()
        {
            var controller = new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandDetailViewController>(ViewType.DetailView,new ModelDifferenceObject(Session.DefaultSession));
            bool created = false;
            Isolate.WhenCalled(() => controller.CreatePopupListView(new ShowViewParameters())).DoInstead(context => created=true);
            var combineAction = controller.CombineAction;
            combineAction.Active.Clear();

            combineAction.DoExecute();

            Assert.IsTrue(created);
        }

        [Test]
        [Isolated]
        public void Popup_ListView_Is_Modal()
        {
            var controller = new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandDetailViewController>(ViewType.DetailView,new ModelDifferenceObject(Session.DefaultSession));
            Isolate.WhenCalled(() => controller.GetCollectionSource()).ReturnRecursiveFake();
            var parameters = new ShowViewParameters();

            controller.CreatePopupListView(parameters);

            Assert.IsNotNull(parameters.CreatedView);
            Assert.AreEqual(TemplateContext.PopupWindow, parameters.Context);
            Assert.AreEqual(TargetWindow.NewModalWindow, parameters.TargetWindow);
        }
        [Test]
        [Isolated]
        public void Popup_ListView_Collection_Will_contain_All_Difference_Objects_With_Same_Application_As_CurrentDifference_Except_Current()
        {
            var currentModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication =new PersistentApplication(Session.DefaultSession) { Name = "AppName" }, };
            currentModelDifferenceObject.Save();
            var expectedModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { Name = "AppName" } };
            expectedModelDifferenceObject.Save();
            new ModelDifferenceObject(Session.DefaultSession).Save();
            var controller = new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandDetailViewController>(ViewType.DetailView, currentModelDifferenceObject);

            var collectionSourceBase = controller.GetCollectionSource();

            Assert.AreEqual(1, collectionSourceBase.GetCount());
            Assert.AreEqual(expectedModelDifferenceObject.Oid, ((ModelDifferenceObject)collectionSourceBase.Collection[0]).Oid);
        }
        [Test]
        [Isolated]
        public void Popup_ListView_Collection_Will_contain_All_Difference_Objects_With_Same_Difference_As_CurrentDifference_Except_Current()
        {
            var currentModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { Name = "AppName" } };
            currentModelDifferenceObject.Save();
            var expectedModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { Name = "AppName" } };
            expectedModelDifferenceObject.Save();
            new ModelDifferenceObject(Session.DefaultSession).Save();
            var controller = new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandDetailViewController>(ViewType.DetailView, currentModelDifferenceObject);

            var collectionSourceBase = controller.GetCollectionSource();

            Assert.AreEqual(1, collectionSourceBase.GetCount());
            Assert.AreEqual(expectedModelDifferenceObject.Oid, ((ModelDifferenceObject)collectionSourceBase.Collection[0]).Oid);
        }
        [Test]
        [Isolated]
        public void When_Selected_Objects_Accepted_Combine_With_Current_Object(){
            var currentModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){Model = DefaultDictionary,PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            var controller = new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandDetailViewController>(ViewType.DetailView, currentModelDifferenceObject);
            controller.DialogController.AcceptAction.Active.Clear();
            var args = Isolate.Fake.InstanceAndSwapAll<SimpleActionExecuteEventArgs>();
            Isolate.WhenCalled(() => args.SelectedObjects).
                WillReturn(new List<ModelDifferenceObject>{new ModelDifferenceObject(Session.DefaultSession)});

            var combiner = Isolate.Fake.InstanceAndSwapAll<DictionaryCombiner>();
            bool combined = false;
            Isolate.WhenCalled(() => combiner.CombineWith(currentModelDifferenceObject)).DoInstead(context => combined=true);

            controller.DialogController.AcceptAction.DoExecute();

            Assert.IsTrue(combined);            
        }
    }
}