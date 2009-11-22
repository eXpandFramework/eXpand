using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
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
    }
}
