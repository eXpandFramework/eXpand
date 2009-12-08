using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class OnDemand_At_Difference_ListView : OnDemand_At_All_Views<CombineDifferencesOnDemandDetailViewController>{
        [Test(Order = 2)]
        [Isolated]
        [ExpectedException(typeof (UserFriendlyException))]
        public void SelectedDifferenceObjects_Should_Be_Of_The_Same_Application(){
            var controller =
                new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandListViewController>(
                    typeof (ModelDifferenceObject));


            controller.CheckObjectCompatibility(new List<ModelDifferenceObject>{
                                                                                   new UserModelDifferenceObject(
                                                                                       Session.DefaultSession){
                                                                                                                  PersistentApplication =
                                                                                                                      new PersistentApplication(
                                                                                                                      Session.DefaultSession)
                                                                                                              },
                                                                                   new UserModelDifferenceObject(
                                                                                       Session.DefaultSession){
                                                                                                                  PersistentApplication =
                                                                                                                      new PersistentApplication(
                                                                                                                      Session.DefaultSession)
                                                                                                                      {UniqueName = "appName"}
                                                                                                              }
                                                                               });
        }

        [Test]
        [Isolated]
        public void Combination_Can_Take_Place_Only_If_One_Difference_Selected(){
            var controller =
                new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandListViewController>(
                    typeof (ModelDifferenceObject));


            Assert.AreEqual(SelectionDependencyType.RequireSingleObject,
                            controller.CombineSimpleAction.SelectionDependencyType);
        }

        [Test(Order = 4)]
        [Isolated]
        public void ActiveApplicationModelDifference_For_selectedObjects_Must_have_Same_Application_Name(){
            var controller = new ViewControllerFactory().CreateAndActivateController
                <CombineDifferencesOnDemandListViewController>(
                typeof (ModelDifferenceObject));


            var expectedActiveModelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){
                                                                                                           PersistentApplication =
                                                                                                               new PersistentApplication(Session.DefaultSession)
                                                                                                               {UniqueName = "appName"}
                                                                                                       };
            expectedActiveModelDifferenceObject.Save();

            ModelDifferenceObject modelDifferenceObject =
                controller.GetActiveApplicationModelDifference(new List<ModelDifferenceObject>{
                                                                                                  new ModelDifferenceObject(Session.DefaultSession){
                                                                                                                                                       PersistentApplication =
                                                                                                                                                           new PersistentApplication(
                                                                                                                                                           Session.DefaultSession){UniqueName = "appName"}
                                                                                                                                                   }
                                                                                              });

            Assert.AreEqual(expectedActiveModelDifferenceObject.Oid, modelDifferenceObject.Oid);
        }

        [Test(Order = 5)]
        [Isolated]
        public void Confirm_Before_Combine(){
            var controller =
                new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandListViewController>(
                    typeof (ModelDifferenceObject));

            Assert.IsTrue(!string.IsNullOrEmpty(controller.CombineSimpleAction.ConfirmationMessage));
        }

        [Test(Order = 5)]
        [Isolated]
        public void If_Differences_Are_Compatible_Only_Then_Allow_Combination(){
            var controller =
                new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandListViewController>(
                    typeof (ModelDifferenceObject));
            Isolate.WhenCalled(() => controller.CheckObjectCompatibility(null)).IgnoreCall();
            bool combineAndSaved = false;
            Isolate.WhenCalled(() => controller.CombineAndSave(null)).DoInstead(context => combineAndSaved = true);
            controller.CombineSimpleAction.Active.Clear();

            controller.CombineSimpleAction.DoExecute();

            Assert.IsTrue(combineAndSaved);
        }

        [Test(Order = 6)]
        [Isolated]
        public void Combine_And_Save_ActiveApplicationModel_With_SelectedDifferenceObjects(){
            var controller =
                new ViewControllerFactory().CreateAndActivateController<CombineDifferencesOnDemandListViewController>(
                    ViewType.ListView,
                    new ModelDifferenceObject(Session.DefaultSession){
                                                                         Model = DefaultDictionary,
                                                                         PersistentApplication =
                                                                             new PersistentApplication(Session.DefaultSession) { Model = new Dictionary(Schema.GetCommonSchema()), Name = "appName" }
                                                                     });
            var currentObject = (ModelDifferenceObject) controller.View.CurrentObject;
            Isolate.WhenCalled(() => currentObject.GetCombinedModel()).WillReturn(DefaultDictionary);
            Isolate.WhenCalled(() => controller.GetActiveApplicationModelDifference(null)).WillReturn(currentObject);

            controller.CombineAndSave(new List<ModelDifferenceObject>{
                                                                         new ModelDifferenceObject(
                                                                             Session.DefaultSession){
                                                                                                        PersistentApplication
                                                                                                            =
                                                                                                            new PersistentApplication
                                                                                                            (Session.
                                                                                                                 DefaultSession){
                                                                                                                                    Name ="appName",
                                                                                                                                    Model = new Dictionary(Schema.GetCommonSchema())
                                                                                                                                },
                                                                                                        Model =DefaultDictionary2
                                                                                                    }
                                                                     });

            Assert.AreEqual(1, Session.DefaultSession.GetCount<ModelDifferenceObject>(), "not saved");
            Assert.IsNotNull(new ApplicationNodeWrapper(currentObject.Model).BOModel.FindClassByName("MyClass2"),"not combined");
        }
    }
}