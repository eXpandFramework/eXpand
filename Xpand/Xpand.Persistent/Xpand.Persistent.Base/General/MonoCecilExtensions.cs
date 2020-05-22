using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General{
    public static class MonoCecilExtensions{
        public static Assembly FindAssembly(this AppDomain appDomain,AssemblyDefinition assemblyDefinition){
            return appDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.FullName == assemblyDefinition.FullName);
        }

        public static Type FindType(this AppDomain appDomain, TypeDefinition typeDefinition){
            return appDomain.FindAssembly(typeDefinition).GetType(typeDefinition.FullName);
        }

        public static Assembly FindAssembly(this AppDomain appDomain,TypeDefinition typeDefinition){
            return appDomain.FindAssembly(typeDefinition.Module.Assembly);

        }
        static readonly ConcurrentDictionary<Assembly,AssemblyDefinition> AssemblyDefinitions=new ConcurrentDictionary<Assembly, AssemblyDefinition>();
        public static IEnumerable<AssemblyDefinition> ToAssemblyDefinition(this IEnumerable<Assembly> assemblies){
            return assemblies
                .Where(assembly =>!assembly.IsDynamic()&& File.Exists(assembly.Location))
                .Select(assembly => {
                    if (!AssemblyDefinitions.ContainsKey(assembly))
                        AssemblyDefinitions.TryAdd(assembly, AssemblyDefinition.ReadAssembly(assembly.Location));
                    return AssemblyDefinitions[assembly];
                });
        }

    }
}