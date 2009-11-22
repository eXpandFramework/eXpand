using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public interface ITypeDefineBuilder
    {
        Type Define(IPersistentClassInfo classInfo);
        List<TypeInfo> Define(IList<IPersistentClassInfo> classInfos);
        AssemblyBuilder AssemblyBuilder { get; }
        ModuleBuilder ModuleBuilder { get; }
    }
    public interface ITypesInfo
    {
        Type PersistentTypesInfoType { get; }
        Type ExtendedReferenceMemberInfoType { get; }
        Type ExtendedCollectionMemberInfoType { get; }
        Type ExtendedCoreMemberInfoType { get; }
        Type IntefaceInfoType { get; }
    }

    public interface IAssemblyNameBuilder
    {
        ITypeDefineBuilder WithAssemblyName(string assemblyName);
    }
    public interface IWithTypeBuilder
    {
        IDefinePropertyBuilder With(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder);
    }
    public interface IDefinePropertyBuilder
    {
        List<PropertyBuilder> Define(IPersistentClassInfo persistentClassInfo, Action<PropertyBuilder, INamePrefix> created);
    }
    public interface IAttributeBuilder
    {
        void Define(IEnumerable<IPersistentAttributeInfo> persistentAttributeInfos, Action<CustomAttributeBuilder> afterCreated);

    }

}
