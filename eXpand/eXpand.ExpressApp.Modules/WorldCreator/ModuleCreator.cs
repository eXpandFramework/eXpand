using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator {
    public class ModuleCreator
    {
        public List<Type> DefineModule(List<List<IPersistentClassInfo>> collection)
        {

            var types = new List<Type>();
            var builder = PersistentClassInfoTypeBuilder.BuildClass();
            foreach (var list in collection){
                string assemblyName = list[0].AssemblyName;
                ITypeDefineBuilder typeDefineBuilder = createPersistentClasses(list, builder, assemblyName);
                types.Add(createModule(assemblyName, typeDefineBuilder));
            }
            return types;
        }

        ITypeDefineBuilder createPersistentClasses(List<IPersistentClassInfo> list, IAssemblyNameBuilder builder, string assemblyName)
        {
            ITypeDefineBuilder typeDefineBuilder = builder.WithAssemblyName(assemblyName);
            typeDefineBuilder.Define(list);
            return typeDefineBuilder;
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