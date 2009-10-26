using System.Data;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
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
//            const string metadataFileName = "Metadata.xml";
            const string dataFileName = "Data.mdb";

//            if (File.Exists(metadataFileName)) File.Delete(metadataFileName);
            if (File.Exists(dataFileName)) File.Delete(dataFileName);

//            XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionString(metadataFileName),
//                                    AutoCreateOption.DatabaseAndSchema);
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
            
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };

            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );


            Assert.IsNotNull(dataStorage.Dictionary.GetClassInfo(xpClassInfo.ClassType));
            
        }
        [Test]
        [Isolated]
        public void CreateMember()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var info = new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "FullName",DataType = XPODataType.String};
            persistentClassInfo.OwnMembers.Add(info);

            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );

            XPMemberInfo member = xpClassInfo.FindMember("FullName");
            Assert.IsNotNull(member);
            Assert.AreEqual(typeof(string), member.MemberType);
        }
        [Test]
        [Isolated]
        public void CreateCollection()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var orderInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            var orderClassInfo = Session.DefaultSession.Dictionary.AddClass(orderInfo);
            var info = new PersistentCollectionMemberInfo(Session.DefaultSession,
                                                          new PersistentAssociationAttribute(Session.DefaultSession){ElementType = orderClassInfo.ClassType}) {Name = "Orders"};
            persistentClassInfo.OwnMembers.Add(info);

            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );

            XPMemberInfo member = xpClassInfo.FindMember("Orders");
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
            var classInfo = dataStorage.Dictionary.AddClass(classCustomer);
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "TestClassName" };
            var referenceMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession,
                                                                        new PersistentAssociationAttribute(
                                                                            Session.DefaultSession)
                                                                            {AssociationName = "Customer"})
                                          {Name = "Customer", ReferenceType = classInfo.ClassType};
            persistentClassInfo.OwnMembers.Add(referenceMemberInfo);

            var xpClassInfo = dataStorage.Dictionary.AddClass(persistentClassInfo );

            XPMemberInfo member = xpClassInfo.FindMember("Customer");
            Assert.IsNotNull(member);
            Assert.IsNotNull(member.MemberType);
            var attribute = ((AssociationAttribute) member.FindAttributeInfo(typeof(AssociationAttribute)));
            Assert.IsNotNull(attribute);
            Assert.AreEqual("Customer", attribute.Name);
        }

    }
}