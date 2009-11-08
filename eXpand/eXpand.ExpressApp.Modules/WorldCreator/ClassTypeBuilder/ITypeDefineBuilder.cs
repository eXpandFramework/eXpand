using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public interface ITypeDefineBuilder
    {
        Type Define(IPersistentClassInfo classInfo);
        List<TypeInfo> Define(IList<IPersistentClassInfo> classInfos);
        AssemblyBuilder AssemblyBuilder { get; }
        ModuleBuilder ModuleBuilder { get; }
    }
}