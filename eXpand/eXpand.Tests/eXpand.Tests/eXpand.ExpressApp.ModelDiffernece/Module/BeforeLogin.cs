using System;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.Win;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.Core;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Module{

    [TestFixture]
    public class BeforeLogin:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void On_CreateCustomModelDiffernceStore_ApplicationConnectionString_Should_Be_Stored(){
            var module = new ModelDifferenceWindowsFormsModule();
            var application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal);
            application.ConnectionString = "test";
            using (RecorderManager.StartRecording()){
                application.CreateCustomModelDifferenceStore += null;
            }
            module.Setup(application);
            var eventHandle = (EventHandler<CreateCustomModelDifferenceStoreEventArgs>) RecorderManager.LastMockedEvent.GetEventHandle();
            var args = new CreateCustomModelDifferenceStoreEventArgs();

            eventHandle.Invoke(this,args);

            Assert.IsTrue(args.Handled);
            Assert.AreEqual("test", module.ConnectionString);
        }
        [Test]
        [Isolated]
        public void Create_User_And_Role_DynamicMembers()
        {

            var module = new ModelDifferenceModule();
            bool user = false;
            Isolate.WhenCalled(() => UserDifferenceObjectBuilder.CreateDynamicMembers()).DoInstead(context => user = true);
            bool role = false;
            Isolate.WhenCalled(() => RoleDifferenceObjectBuilder.CreateDynamicMembers()).DoInstead(context => role = true);

            module.CustomizeTypesInfo(XafTypesInfo.Instance);

            Assert.IsTrue(role);
            Assert.IsTrue(user);
        }


        [Test]
        [Isolated]
        public void If_Security_Is_Not_Complex_UserType_Is_Set_Remove_RoleAspect_Node()
        {
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));
            var wrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema()));
            wrapper.Load(typeof(RoleModelDifferenceObject));
            
            var module = new ModelDifferenceModule();
            Isolate.WhenCalled(() => module.AddCultures(null)).IgnoreCall();
         
            module.UpdateModel(wrapper.Dictionary);

            Assert.AreEqual(0, wrapper.BOModel.Classes.Count);

        }

        [Test]
        [Isolated]
        public void When_Updating_Model_It_Should_Add_AllCultures_As_Predifined_Values_Of_CurrentLanguage_Member(){
            var dictionary = DictionaryFactory.Create(typeof(ModelDifferenceObject));
            var module = new ModelDifferenceModule();
            Isolate.WhenCalled(() => module.GetAllCultures()).WillReturn("test");

            module.UpdateModel(dictionary);

            var name = new ModelDifferenceObject(Session.DefaultSession).GetPropertyInfo(x=>x.PreferredAspect).Name;
            var single = new ApplicationNodeWrapper(dictionary).BOModel.FindClassByType(typeof(ModelDifferenceObject)).AllProperties.Where(wrapper => wrapper.Name==name).Single();
            Assert.AreEqual("test", single.Node.GetAttributeValue("PredefinedValues"));
        }
        [Test]
        [Isolated]
        public void DefaultLanguage_IsContained_In_Cultures(){
            var module = new ModelDifferenceModule();

            var cultures = module.GetAllCultures();

            var strings = cultures.Split(';');
            Assert.AreEqual(CultureInfo.GetCultures(CultureTypes.AllCultures).Count()+1, strings.Length);
            Assert.AreEqual(DictionaryAttribute.DefaultLanguage, strings[0]);
        }
    }
}