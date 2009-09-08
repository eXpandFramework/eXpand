using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using System.Linq;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ClonningModelDifferencesObjects{
    [TestFixture]
    public class OnDemand_At_All_Views:eXpandBaseFixture{
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
        [ClearMocks]
        public void Some_Properties_Should_Change(){
            Isolate.Fake.InstanceAndSwapAll<DictionaryNode>();
            DateTime dateTime = DateTime.Now;
            Mock mock = MockManager.Mock(typeof (CloneObjectViewController),Constructor.NotMocked);
            mock.CallBase.ExpectAlways("CloneObject");
            var singleChoiceActionExecuteEventArgs = Isolate.Fake.InstanceAndSwapAll<SingleChoiceActionExecuteEventArgs>();
            var data = new ModelDifferenceObject(Session.DefaultSession){Name = "name"};
            Isolate.WhenCalled(() => singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView.CurrentObject).WillReturn(data);
            var controller = new CloneObjectViewController();
            
            controller.CloneObjectAction.Active.Clear();

            controller.CloneObjectAction.DoExecute(new ChoiceActionItem(new DictionaryNode(""), data));

            Assert.IsNull(data.Name);
            Assert.IsTrue(data.Disabled);
            Assert.GreaterThan(data.DateCreated, dateTime);
            Assert.IsNotNull(data.PersistentApplication);
            
        }

    }
}