using System;
using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.WorldCreator;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace Xpand.Tests.Xpand.Xpo {
    [TestFixture]
    public class PersistentMetaDataFixture {
        private IDataLayer dataStorage;

        private void InitDataLayers() {
            dataStorage = new SimpleDataLayer(new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema));
        }

        [SetUp]
        public void Setup() {
            InitDataLayers();
        }

        [Test]
        [Isolated]
        public void CreateClass() {
            throw new NotImplementedException();

            //            var persistentClassInfo = new PersistentClassInfo(UnitOfWork.DefaultSession) { Name = "TestClassName" };
            //
            //            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );
            //
            //
            //            Assert.IsNotNull(dataStorage.Dictionary.GetClassInfo(xpClassInfo.ClassType));

        }
        [Test]
        [Isolated]
        public void CreateMember() {
            throw new NotImplementedException();
            //            var persistentClassInfo = new PersistentClassInfo(UnitOfWork.DefaultSession) { Name = "TestClassName" };
            //            var info = new PersistentCoreTypeMemberInfo(UnitOfWork.DefaultSession){Name = "FullName",DataType = XPODataType.String};
            //            persistentClassInfo.OwnMembers.Add(info);
            //
            //            var xpClassInfo = UnitOfWork.DefaultSession.Dictionary.AddClass(persistentClassInfo);
            //
            //            XPMemberInfo member = xpClassInfo.FindMember("FullName");
            //            Assert.IsNotNull(member);
            //            Assert.AreEqual(typeof(string), member.MemberType);
        }
        [Test]
        [Isolated]
        public void CreateCollection() {
            throw new NotImplementedException();
            //            var persistentClassInfo = new PersistentClassInfo(UnitOfWork.DefaultSession) { Name = "TestClassName" };
            //            var orderInfo = new PersistentClassInfo(UnitOfWork.DefaultSession) { Name = "Order" };
            //            var orderClassInfo = UnitOfWork.DefaultSession.Dictionary.AddClass(orderInfo);
            //            var info = new PersistentCollectionMemberInfo(UnitOfWork.DefaultSession,
            //                                                          new PersistentAssociationAttribute(UnitOfWork.DefaultSession){ElementType = orderClassInfo.ClassType}) {Name = "Orders"};
            //            persistentClassInfo.OwnMembers.Add(info);
            //
            //            var xpClassInfo = UnitOfWork.DefaultSession.Dictionary.AddClass(persistentClassInfo);
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
        public void CreateReferenceMember() {
            throw new NotImplementedException();
            //            var classCustomer = new PersistentClassInfo(UnitOfWork.DefaultSession) {Name = "Customer"};
            //            var classInfo = dataStorage.Dictionary.AddClass(classCustomer);
            //            var persistentClassInfo = new PersistentClassInfo(UnitOfWork.DefaultSession) { Name = "TestClassName" };
            //            var referenceMemberInfo = new PersistentReferenceMemberInfo(UnitOfWork.DefaultSession,
            //                                                                        new PersistentAssociationAttribute(
            //                                                                            UnitOfWork.DefaultSession)
            //                                                                            {AssociationName = "Customer",ElementType = classInfo.ClassType})
            //                                          {Name = "Customer", ReferenceType = classInfo.ClassType};
            //            persistentClassInfo.OwnMembers.Add(referenceMemberInfo);
            //
            //            var xpClassInfo = UnitOfWork.DefaultSession.Dictionary.AddClass(persistentClassInfo );
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