using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using MbUnit.Framework;
using eXpand.Xpo;

namespace eXpand.Tests.WorldCreator
{
    [Subject(typeof(PersistentTypeInfo))]
    public class When_Saving_PersistentTypes:With_In_Memory_DataStore {
        static PersistentReferenceMemberInfo _referenceMemberInfo;
        Establish context = () => { _referenceMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession) { ReferenceType = typeof(User) }; };
        Because of = () => _referenceMemberInfo.Save();

        It should_persist_the_referenceType =
            () =>
            ((PersistentReferenceMemberInfo) new Session().GetObject(_referenceMemberInfo)).ReferenceType.ShouldEqual(
                typeof (User));
    }
}
