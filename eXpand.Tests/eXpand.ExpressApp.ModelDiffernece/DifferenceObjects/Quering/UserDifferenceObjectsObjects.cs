using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions.eXpand;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Quering{
    [TestFixture]
    public class UserDifferenceObjectsObjects:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void As_ActiveDifferenceObjectss_Should_Return_All_UserDifferenceObjectssObjects_That_Belong_To_ActiveObjects_Collection()
        {
            Isolate.Fake.ISecurityComplex();
            #region register enitties

            XafTypesInfo.Instance.RegisterEntity(typeof(UserModelDifferenceObject));
            new ModelDifferenceModule().CustomizeTypesInfo(null);
            #endregion

            List<UserModelDifferenceObject> userDifferenceObjectsObject1 = GetUserStoreObject();
            List<UserModelDifferenceObject> userDifferenceObjectsObject2 = GetUserStoreObject();
            List<UserModelDifferenceObject> userDifferenceObjectsObject3 = GetUserStoreObject();
            foreach (var DifferenceObjectsObject in userDifferenceObjectsObject3)
            {
                ((XPCollection)DifferenceObjectsObject.GetMemberValue("Users")).Remove(SecuritySystem.CurrentUser);
                DifferenceObjectsObject.Save();
            }

            var query = new QueryUserModelDifferenceObject(Session.DefaultSession);

            IQueryable<UserModelDifferenceObject> store = query.GetActiveModelDifferences( "AppName");

            Assert.AreEqual(4, store.Count());
            Assert.AreElementsEqualIgnoringOrder(userDifferenceObjectsObject1.Concat(userDifferenceObjectsObject2), store.Cast<UserModelDifferenceObject>());


        }
        private List<UserModelDifferenceObject> GetUserStoreObject()
        {
            var objects = new List<UserModelDifferenceObject>();
            var userStoreObject = new UserModelDifferenceObject(Session.DefaultSession) { PersistentApplication =new PersistentApplication(Session.DefaultSession) { Name = "AppName" } };
            ((XPCollection)userStoreObject.GetMemberValue("Users")).Add(SecuritySystem.CurrentUser);
            userStoreObject.Save();
            objects.Add(userStoreObject);

            userStoreObject = new UserModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession){ Name = "AppName" } };
            ((XPCollection)userStoreObject.GetMemberValue("Users")).Add(SecuritySystem.CurrentUser);
            userStoreObject.Save();
            objects.Add(userStoreObject);
            return objects;
        }

    }
}