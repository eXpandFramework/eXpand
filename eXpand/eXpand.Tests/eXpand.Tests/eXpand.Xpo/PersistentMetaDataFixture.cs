using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.Xpo.PersistentMetaData;
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
//            const string metadataFileName = "Metadata.xml";
            const string dataFileName = "Data.mdb";

//            if (File.Exists(metadataFileName)) File.Delete(metadataFileName);
            if (File.Exists(dataFileName)) File.Delete(dataFileName);

//            XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionString(metadataFileName),
//                                    AutoCreateOption.DatabaseAndSchema);
            dataStorage = XpoDefault.GetDataLayer(AccessConnectionProvider.GetConnectionString(dataFileName),
                                                  AutoCreateOption.DatabaseAndSchema);
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

            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };

            dataStorage.Dictionary.AddClasses(new List<PersistentClassInfo> { persistentClassInfo });
            

            Assert.IsNotNull(dataStorage.Dictionary.GetClassInfo("", persistentClassInfo.Name));
            throw new UserFriendlyException(new Exception("your message"));
        }
        [Test]
        [Isolated]
        public void CreateMember()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var info = new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "FullName",TypeName = typeof(string).FullName};
            persistentClassInfo.OwnMembers.Add(info);

            dataStorage.Dictionary.AddClasses(new List<PersistentClassInfo> { persistentClassInfo });

            XPMemberInfo member = dataStorage.Dictionary.GetClassInfo("",persistentClassInfo.Name).FindMember("FullName");
            Assert.IsNotNull(member);
            Assert.AreEqual(typeof(string), member.MemberType);
        }
        [Test]
        [Isolated]
        public void CreateCollection()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var info = new PersistentCollectionMemberInfo(Session.DefaultSession,
                                                          new PersistentAssociationAttribute(Session.DefaultSession)
                                                              {ElementTypeName = "Order"}) {Name = "Orders"};
            persistentClassInfo.OwnMembers.Add(info);

            dataStorage.Dictionary.AddClasses(new List<PersistentClassInfo> { persistentClassInfo });

            XPMemberInfo member = dataStorage.Dictionary.GetClassInfo("",persistentClassInfo.Name).FindMember("Orders");
            Assert.IsNotNull(member);
            Assert.AreEqual(typeof(XPCollection), member.MemberType);
            var attribute = ((AssociationAttribute) member.FindAttributeInfo(typeof(AssociationAttribute)));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(attribute.ElementTypeName, "Order");
        }
        [Test]
        [Isolated]
        public void CreateReferenceMember()
        {
            var classCustomer = new PersistentClassInfo(Session.DefaultSession) {Name = "Customer"};
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var referenceMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession,
                                                                        new PersistentAssociationAttribute(
                                                                            Session.DefaultSession)
                                                                            {AssociationName = "Customer"})
                                          {Name = "Customer", ReferenceType = classCustomer};
            persistentClassInfo.OwnMembers.Add(referenceMemberInfo);

            dataStorage.Dictionary.AddClasses(new List<PersistentClassInfo> { persistentClassInfo });

            XPMemberInfo member = dataStorage.Dictionary.GetClassInfo("",persistentClassInfo.Name).FindMember("Customer");
            Assert.IsNotNull(member);
            Assert.IsNull(member.MemberType);
            var attribute = ((AssociationAttribute) member.FindAttributeInfo(typeof(AssociationAttribute)));
            Assert.IsNotNull(attribute);
            Assert.AreEqual("Customer", attribute.Name);
        }

    }
}