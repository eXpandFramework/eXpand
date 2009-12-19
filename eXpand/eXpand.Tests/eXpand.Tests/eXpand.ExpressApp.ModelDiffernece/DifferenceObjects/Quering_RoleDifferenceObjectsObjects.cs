using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Quering_RoleDifferenceObjectsObjects:XpandBaseFixture
    {
        [Test]
        public void ActiveRoleDifferenceObjectss_For_User_That_Do_Not_Have_Roles_Should_Be_Zero(){
            var queryRoleDifferenceObjectsObject = new QueryRoleModelDifferenceObject(Session.DefaultSession);

            var queryable = queryRoleDifferenceObjectsObject.GetActiveModelDifferences("");

            Assert.AreEqual(0, queryable.Count());
        }

        [Test]
        [Isolated]
        public void ActiveDifferenceObjectsRoles_Will_Be_All_ModelDifferenceObjectsObjects_that_are_OfRoleType_And_That_At_Leat_One_of_Their_Roles_Is_Current_User_Role()
        {
            Isolate.Fake.ISecurityComplex();
            var role = new Role(Session.DefaultSession);
            role.Save();
            ((User)SecuritySystem.CurrentUser).Roles.Add(role);
            XafTypesInfo.Instance.RegisterEntity(typeof(RoleModelDifferenceObject));
            RoleDifferenceObjectBuilder.CreateDynamicMembers(TODO);
            new ModelDifferenceObject(Session.DefaultSession){PersistentApplication = new PersistentApplication(Session.DefaultSession)}.Save();
            var roleDifferenceObjectsObject1 = new RoleModelDifferenceObject(Session.DefaultSession) { PersistentApplication =new PersistentApplication(Session.DefaultSession) { UniqueName = "AppName" } };
            ((XPCollection)roleDifferenceObjectsObject1.GetMemberValue("Roles")).Add(role);
            roleDifferenceObjectsObject1.Save();
            var roleDifferenceObjectsObject2 = new RoleModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { UniqueName = "AppName" } };
            roleDifferenceObjectsObject2.Save();
            ((XPCollection)roleDifferenceObjectsObject2.GetMemberValue("Roles")).Add(role);
            new RoleModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession){ Name = "AppName" } }.Save();
            

            IQueryable<RoleModelDifferenceObject> queryable = new QueryRoleModelDifferenceObject(Session.DefaultSession).GetActiveModelDifferences("AppName");

            Assert.AreEqual(2, queryable.Count());
        }

    }
}