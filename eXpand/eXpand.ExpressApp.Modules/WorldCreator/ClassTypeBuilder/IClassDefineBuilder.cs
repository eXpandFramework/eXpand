using System;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public interface IClassDefineBuilder
    {
        Type Define(IPersistentClassInfo classInfo);
    }
}