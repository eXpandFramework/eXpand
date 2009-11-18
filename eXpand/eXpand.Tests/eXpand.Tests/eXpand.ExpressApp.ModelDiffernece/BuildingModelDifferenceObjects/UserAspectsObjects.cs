using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions.eXpand;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.BuildingModelDifferenceObjects{
    [TestFixture]
    public class UserAspectsObjects:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void A_Many_To_Many_Association_With_User_Should_Be_Created_If_User_Exists()
        {
            Isolate.Fake.ISecurityComplex();
            ITypesInfo instance = XafTypesInfo.Instance;
            instance.RegisterEntity(typeof(User));


            bool members = UserDifferenceObjectBuilder.CreateDynamicMembers(null);

            Assert.IsTrue(members);


        }
        [Test]
        [Isolated]
        public void Can_Be_Assign_Aspect_To_Current_User()
        {
            Isolate.Fake.ISecurityComplex();
            XafTypesInfo.Instance.RegisterEntity(typeof(UserModelDifferenceObject));
            UserDifferenceObjectBuilder.CreateDynamicMembers(null);


            var userAspectObject = new UserModelDifferenceObject(Session.DefaultSession);
            
            userAspectObject.AssignToCurrentUser();

            Assert.AreEqual(1, ((XPCollection) userAspectObject.GetMemberValue("Users")).Count);
        }

    }
}