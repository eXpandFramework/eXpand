using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class ModuleCreator
    {
        public List<Type> DefineModule(List<List<IPersistentClassInfo>> collection)
        {
//            var builder = PersistentClassInfoTypeBuilder.BuildClass();
            return (from list in collection
                    let assemblyName = list[0].AssemblyName
                    let typeDefineBuilder = createPersistentClasses(list,  assemblyName)
                    select createModule(assemblyName, typeDefineBuilder)).ToList();
        }

        ITypeDefineBuilder createPersistentClasses(List<IPersistentClassInfo> list, string assemblyName)
        {
//            ITypeDefineBuilder typeDefineBuilder = builder.WithAssemblyName(assemblyName);
//            typeDefineBuilder.Define(list);
//            return typeDefineBuilder;
        }

        Type createModule(string assemblyName, ITypeDefineBuilder typeDefineBuilder)
        {
            var typeBuilder = typeDefineBuilder.ModuleBuilder.DefineType(assemblyName + ".Module",
                                                                         TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit,
                                                                         typeof(ModuleBase));
            return typeBuilder.CreateType();
        }
    }
}