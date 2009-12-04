using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.WorldCreator {
    public abstract class With_Types : With_In_Memory_DataStore
    {
        protected static TypesInfo TypesInfo;

        Establish context = () =>
        {
            TypesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.WhenCalled(() => TypesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedReferenceMemberInfoType).WillReturn(typeof(ExtendedReferenceMemberInfo));
            Isolate.WhenCalled(() => TypesInfo.ExtendedCollectionMemberInfoType).WillReturn(typeof(ExtendedCollectionMemberInfo));
        };
    }
}