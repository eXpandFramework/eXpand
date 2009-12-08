using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using System.Linq;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ClonningModelDifferencesObjects{
    [TestFixture]
    public class OnDemand_At_All_Views:XpandBaseFixture{
        [Test]
        [Isolated]
        public void Can_Be_Cloned(){
            
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)};

            var xpSimpleObject = new Cloner().CloneTo(modelDifferenceObject, typeof (ModelDifferenceObject));

            Assert.IsNotNull(xpSimpleObject);
        }

        [Isolated]
        [Test(Order = 10)]
        public void All_DifferenceObjectTypes_Are_CloneAble(){
            ITypeInfo info = XafTypesInfo.Instance.FindTypeInfo(typeof(ModelDifferenceObject));

            CustomAttribute customAttribute = info.FindAttributes<CustomAttribute>().Where(attribute => attribute.Name == "IsClonable" && attribute.Value == "True").FirstOrDefault();

            Assert.IsNotNull(customAttribute);
        }

        [Test(Order = 11)]
        [Isolated]
        public void Some_Properties_Should_Change(){
            DateTime dateTime = DateTime.Now;
            Mock mock = MockManager.Mock(typeof (CloneObjectViewController),Constructor.NotMocked);
            mock.CallBase.ExpectAlways("CloneObject");
            var singleChoiceActionExecuteEventArgs = Isolate.Fake.InstanceAndSwapAll<SingleChoiceActionExecuteEventArgs>();
            var data = new ModelDifferenceObject(Session.DefaultSession){Name = "name"};
            Isolate.WhenCalled(() => singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView.CurrentObject).WillReturn(data);
            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            var controller = new ViewControllerFactory().CreateController<CloneObjectViewController>(ViewType.DetailView, new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = persistentApplication});
            controller.CloneObjectAction.Active.Clear();
            
            controller.CloneObjectAction.DoExecute(new ChoiceActionItem(new DictionaryNode(""), data));

            Assert.IsNull(data.Name);
            Assert.IsTrue(data.Disabled);
            Assert.GreaterThan(data.DateCreated, dateTime);
            Assert.AreEqual(persistentApplication, data.PersistentApplication);
            
        }
//        [Test]
//        [Isolated]
//        public void When_Cloning_Object_Difference_Detail_View_Should_Be_Created(){
//            Mock mock = MockManager.Mock(typeof(CloneObjectViewController), Constructor.NotMocked);
//            mock.CallBase.ExpectAlways("CloneObject");
//            var controller = new CloneObjectViewController();
//            bool created = false;
//            Isolate.WhenCalled(() => controller.CreateDifferenceTypeDetailView(null)).DoInstead(context => created = true);
//            controller.CloneObjectAction.Active.Clear();
//
//            controller.CloneObjectAction.DoExecute(new ChoiceActionItem());
//
//            Assert.IsTrue(created);
//        }
//        [Test]
//        [Isolated]
//        public void DiffenceDetailView_Should_Create_A_DialodController(){
//            Mock mock = MockManager.Mock(typeof(CloneObjectViewController), Constructor.NotMocked);
//            mock.CallBase.ExpectAlways("OnActivated");
//            var controller = new ViewControllerFactory().CreateAndActivateController<CloneObjectViewController>(ViewType.Any,new ModelDifferenceObject(Session.DefaultSession));
//            var singleChoiceActionExecuteEventArgs = Isolate.Fake.Instance<SingleChoiceActionExecuteEventArgs>(Members.CallOriginal);
//            
//            controller.CreateDifferenceTypeDetailView(singleChoiceActionExecuteEventArgs);
//
//            Assert.AreEqual(1, singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Count);
//        }

//        [Test]
//        [Isolated]
//        public void When_Accepting_DifferenceType_Object_Should_Clone_View_Current_Object_For_Selected_DifferenceType(){
//            Mock mock = MockManager.Mock(typeof(CloneObjectViewController), Constructor.NotMocked);
//            mock.CallBase.ExpectAlways("OnActivated");
//            var controller = new ViewControllerFactory().CreateAndActivateController<CloneObjectViewController>(ViewType.Any, new ModelDifferenceObject(Session.DefaultSession));
//            var singleChoiceActionExecuteEventArgs = Isolate.Fake.Instance<SingleChoiceActionExecuteEventArgs>(Members.CallOriginal);
//            controller.CreateDifferenceTypeDetailView(singleChoiceActionExecuteEventArgs);
//            controller.DialogController.AcceptAction.Active.Clear();
//            var args = Isolate.Fake.InstanceAndSwapAll<SimpleActionExecuteEventArgs>(Members.CallOriginal);
//            Isolate.WhenCalled(() => args.CurrentObject).WillReturn(new DifferenceTypeObject{DifferenceType = DifferenceType.Role});
//            var roleModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<RoleModelDifferenceObject>();
//            var cloner = Isolate.Fake.InstanceAndSwapAll<Cloner>();
//            object parameter = null;
//            Isolate.WhenCalled(() => cloner.CloneTo(null, null)).DoInstead(context =>{
//                                                                               parameter = context.Parameters[1];
//                                                                               return roleModelDifferenceObject;
//                                                                           });
//
//            controller.DialogController.AcceptAction.DoExecute();
//
//
//            Assert.AreEqual(typeof(RoleModelDifferenceObject), parameter);
//        }
    }
}