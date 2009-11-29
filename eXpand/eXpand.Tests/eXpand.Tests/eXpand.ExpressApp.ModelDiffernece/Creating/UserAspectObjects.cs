using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions.eXpand;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Creating
{
    [TestFixture]
    public class UserAspectObjects:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void DifferenceType_Is_Model()
        {
            var o = new UserModelDifferenceObject(Session.DefaultSession);

            Assert.AreEqual(DifferenceType.User, o.DifferenceType);
        }

        [Test]
        [Isolated]
        public void When_Initilize_Members_Name_Should_Initialize_Application(){
            Isolate.Fake.ISecurityComplex();
            var aspectObject = new UserModelDifferenceObject(Session.DefaultSession);

            aspectObject.InitializeMembers("name","");

            Assert.IsNotNull(aspectObject.PersistentApplication);
            Assert.AreEqual("name", aspectObject.PersistentApplication.Name);
        }
        [Test]
        [Isolated]
        public void When_Initilize_Members_Name_Should_Contain_Current_User_Name()
        {
            Isolate.Fake.ISecurityComplex();
            var aspectObject = new UserModelDifferenceObject(Session.DefaultSession);
            ((User)SecuritySystem.CurrentUser).UserName = "UserName";

            aspectObject.InitializeMembers("", "");

            Assert.IsTrue(aspectObject.Name.IndexOf("UserName") > -1);
        }
    }
}
