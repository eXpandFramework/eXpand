using System;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public interface ITypesInfo {
        Type PersistentTypesInfoType { get; }
        Type ExtendedReferenceMemberInfoType { get; }
        Type ExtendedCollectionMemberInfoType { get; }
        Type ExtendedCoreMemberInfoType { get; }
        Type IntefaceInfoType { get; }
    }
}