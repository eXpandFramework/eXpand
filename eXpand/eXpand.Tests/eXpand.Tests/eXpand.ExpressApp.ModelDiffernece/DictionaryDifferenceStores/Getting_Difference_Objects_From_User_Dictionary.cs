using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using TypeMock.Extensions.eXpand;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores{
    [TestFixture]
    public class Getting_Difference_Objects_From_User_Dictionary : XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void As_Active_UserDifference_Will_Return_Active_UserDifferenceObject()
        {

            var store = new XpoUserModelDictionaryDifferenceStore( Isolate.Fake.Instance<XafApplication>());
            Isolate.Fake.StaticMethods(typeof(UserDifferenceObjectBuilder));
            var userStoreObject = new UserModelDifferenceObject(Session.DefaultSession);


            var queryUserDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryUserModelDifferenceObject>();
            Isolate.WhenCalled(() => queryUserDifferenceObject.GetActiveModelDifference("")).WillReturn(userStoreObject);

            ModelDifferenceObject modelDifferenceObject = store.GetActiveDifferenceObject();

            Assert.AreEqual(userStoreObject, modelDifferenceObject);
        }
        [Test]
        [Isolated][MultipleAsserts]
        public void As_ActiveUserDifferences_Will_Return_A_Concat_Of_Active_UserDifferenceObjects_And_RoleDifferenceObjects()
        {
            var store = new XpoUserModelDictionaryDifferenceStore( Isolate.Fake.Instance<XafApplication>());
            var userDifferenceObject1 = new UserModelDifferenceObject(Session.DefaultSession);

            var queryUserDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryUserModelDifferenceObject>();
            Isolate.WhenCalled(() => queryUserDifferenceObject.GetActiveModelDifferences("")).WillReturn(new List<UserModelDifferenceObject> { userDifferenceObject1 }.AsQueryable());
            var roleDifferenceObject = new RoleModelDifferenceObject(Session.DefaultSession);

            var queryRoleDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryRoleModelDifferenceObject>();
            Isolate.WhenCalled(() => queryRoleDifferenceObject.GetActiveModelDifferences("")).WillReturn(new List<RoleModelDifferenceObject> { roleDifferenceObject }.AsQueryable());


            IQueryable<ModelDifferenceObject> queryable = store.GetActiveDifferenceObjects();

            Assert.AreEqual(2, queryable.Count());
            Assert.AreEqual(userDifferenceObject1, queryable.ToList()[1]);
            Assert.AreEqual(roleDifferenceObject, queryable.ToList()[0]);
        }
        [Test]
        [Isolated]
        public void When_A_New_DifferenceObject_Is_Requested_Will_Return_A_UserDifferenceObject()
        {
            Isolate.Fake.ISecurityComplex();
            XafTypesInfo.Instance.RegisterEntity(typeof(UserModelDifferenceObject));
            UserDifferenceObjectBuilder.CreateDynamicMembers();
            
            
            var store = new XpoUserModelDictionaryDifferenceStore( Isolate.Fake.Instance<XafApplication>());

            
            ModelDifferenceObject modelDifferenceObject = store.GetNewDifferenceObject(
                                                                                       Isolate.Fake.Instance<ObjectSpace>());

            Assert.IsTrue(modelDifferenceObject.IsNewObject);
            var collection = (XPCollection)modelDifferenceObject.GetMemberValue("Users");
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(SecuritySystem.CurrentUser, collection[0]);
        }

    }
}