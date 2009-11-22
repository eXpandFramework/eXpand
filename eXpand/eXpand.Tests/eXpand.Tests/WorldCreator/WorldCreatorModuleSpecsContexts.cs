using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;
using TypesInfo = eXpand.ExpressApp.WorldCreator.Core.TypesInfo;

namespace eXpand.Tests.WorldCreator
{
    public abstract class with_TypesInfo
    {
        Establish context = () =>
        {
            typesInfo = Isolate.Fake.Instance<TypesInfo>();
            Isolate.Swap.NextInstance<TypesInfo>().With(typesInfo);
            Isolate.WhenCalled(() => typesInfo.ExtendedCollectionMemberInfoType).WillReturn(typeof(ExtendedCollectionMemberInfo));
            Isolate.WhenCalled(() => typesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof(ExtendedCoreTypeMemberInfo));
            Isolate.WhenCalled(() => typesInfo.ExtendedReferenceMemberInfoType).WillReturn(typeof(ExtendedReferenceMemberInfo));
            Isolate.WhenCalled(() => typesInfo.IntefaceInfoType).WillReturn(typeof(InterfaceInfo));
            Isolate.WhenCalled(() => typesInfo.PersistentTypesInfoType).WillReturn(typeof(PersistentClassInfo));
        };

        protected static TypesInfo typesInfo;
    }
}
