using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using MbUnit.Framework;
using eXpand.Xpo;
using eXpand.ExpressApp.WorldCreator;

namespace eXpand.Tests.WorldCreator
{
    [TestFixture]
    public class When_Saving_PersistentTypes:XpandBaseFixture
    {
        [Test]
        public void ReferenceMemberInfo_Reference_Type_Can_Persist() {
            var info = new PersistentReferenceMemberInfo(Session.DefaultSession) {ReferenceType = GetType()};
            info.Save();
            
            var o = (PersistentReferenceMemberInfo) new Session().GetObject(info);

            Assert.AreEqual(GetType(), o.ReferenceType);
        }
        [Test]
        public void Test_That_No_Multi_Assemblies_With_Same_Name_Are_Created() {
            var customerClassInfo = new PersistentClassInfo(Session.DefaultSession){Name = "Customer"};
            var orderClassInfo = new PersistentClassInfo(Session.DefaultSession){Name = "Order"};

            Session.DefaultSession.Dictionary.AddClasses(new List<IPersistentClassInfo> { orderClassInfo,customerClassInfo });

            Assert.IsNotNull(Type.GetType(customerClassInfo.PersistentTypeClassInfo.ClassType.AssemblyQualifiedName));
            Assert.IsNotNull(Type.GetType(orderClassInfo.PersistentTypeClassInfo.ClassType.AssemblyQualifiedName));
        }
    }
}
