using System;
using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.Xpo
{
    [TestFixture]
    public class PersistentMetaDataFixture
    {
        private IDataLayer dataStorage;

        private void InitDataLayers()
        {
            dataStorage = new SimpleDataLayer(new InMemoryDataStore(new DataSet(),AutoCreateOption.DatabaseAndSchema ));
        }

        [SetUp]
        public void Setup()
        {
            InitDataLayers();
        }

        [Test]
        [Isolated]
        public void CreateClass()
        {
            throw new NotImplementedException();
            
//            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
//
//            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );
//
//
//            Assert.IsNotNull(dataStorage.Dictionary.GetClassInfo(xpClassInfo.ClassType));
            
        }
        [Test]
        [Isolated]
        public void CreateMember()
        {
            throw new NotImplementedException();
//            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
//            var info = new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "FullName",DataType = XPODataType.String};
//            persistentClassInfo.OwnMembers.Add(info);
//
//            var xpClassInfo = Session.DefaultSession.Dictionary.AddClass(persistentClassInfo);
//
//            XPMemberInfo member = xpClassInfo.FindMember("FullName");
//            Assert.IsNotNull(member);
//            Assert.AreEqual(typeof(string), member.MemberType);
        }
        [Test]
        [Isolated]
        public void CreateCollection()
        {
            throw new NotImplementedException();
//            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
//            var orderInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
//            var orderClassInfo = Session.DefaultSession.Dictionary.AddClass(orderInfo);
//            var info = new PersistentCollectionMemberInfo(Session.DefaultSession,
//                                                          new PersistentAssociationAttribute(Session.DefaultSession){ElementType = orderClassInfo.ClassType}) {Name = "Orders"};
//            persistentClassInfo.OwnMembers.Add(info);
//
//            var xpClassInfo = Session.DefaultSession.Dictionary.AddClass(persistentClassInfo);
//
//            XPMemberInfo member = xpClassInfo.FindMember("Orders");
//            Assert.IsNotNull(member);
//            Assert.AreEqual(typeof(XPCollection), member.MemberType);
//            var attribute = ((AssociationAttribute) member.FindAttributeInfo(typeof(AssociationAttribute)));
//            Assert.IsNotNull(attribute);
//            Assert.AreEqual("WorldCreator.Order", attribute.ElementTypeName);
        }
        [Test]
        [Isolated]
        public void CreateReferenceMember()
        {
            throw new NotImplementedException();
//            var classCustomer = new PersistentClassInfo(Session.DefaultSession) {Name = "Customer"};
//            var classInfo = dataStorage.Dictionary.AddClass(classCustomer);
//            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
//            var referenceMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession,
//                                                                        new PersistentAssociationAttribute(
//                                                                            Session.DefaultSession)
//                                                                            {AssociationName = "Customer",ElementType = classInfo.ClassType})
//                                          {Name = "Customer", ReferenceType = classInfo.ClassType};
//            persistentClassInfo.OwnMembers.Add(referenceMemberInfo);
//
//            var xpClassInfo = Session.DefaultSession.Dictionary.AddClass(persistentClassInfo );
//
//            XPMemberInfo member = xpClassInfo.FindMember("Customer");
//            Assert.IsNotNull(member);
//            Assert.IsNotNull(member.MemberType);
//            var attribute = ((AssociationAttribute) member.FindAttributeInfo(typeof(AssociationAttribute)));
//            Assert.IsNotNull(attribute);
//            Assert.AreEqual("Customer", attribute.Name);
        }

    }
}