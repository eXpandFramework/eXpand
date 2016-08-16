using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Fasterflect;
using Xpand.Utils.Linq;

namespace Xpand.Persistent.Base.General {
    public class ModuleActivator {
        private static string _path;
        private static HashSet<string> _loadedAssemblyNames;

        public static IEnumerable<ModuleBase> CreateInstances(string path,string tabNameFilter,string searchPattern="Xpand.ExpressApp*.dll"){
            _path = path;
            AppDomain.CurrentDomain.AssemblyResolve+=CurrentDomainOnAssemblyResolve;
            _loadedAssemblyNames = new HashSet<string>(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().FullName));
            var filteredTypes = GetModuleTypes(path, tabNameFilter,searchPattern);
            var filteredInstances =filteredTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>().ToArray();
            var requiredModuleTypes = filteredInstances.GetItems<ModuleBase>(m => m.RequiredModuleTypes).Distinct().Select(m => m.GetType());
            var agnosticTypes =GetModuleTypes(path,"Win-Web",searchPattern).Except(requiredModuleTypes);
            filteredInstances= filteredInstances.Concat(agnosticTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>()).ToArray();
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            return filteredInstances;
        }


        private static IEnumerable<Type> GetModuleTypes(string path, string filter,string searchPattern){
            foreach (var file in Directory.GetFiles(path, searchPattern)){
                var assembly = Assembly.LoadFile(file);
                _loadedAssemblyNames.Add(assembly.GetName().FullName);
                var assemblies = new[]{assembly}.GetItems<Assembly>(assembly1 => assembly1.GetReferencedAssemblies().Select(LoadAssembly)).Where(assembly1 => assembly1!=null).Select(assembly1 => assembly1.GetName()).ToArray();
                LoadReferences(assemblies);
                var moduleType =assembly.GetTypes().FirstOrDefault(type => type.GetCustomAttributes(typeof(ToolboxTabNameAttribute), false).Any());
                if (moduleType != null){
                    var toolboxAttribute =moduleType.GetCustomAttributes(typeof(ToolboxTabNameAttribute), false).Cast<ToolboxTabNameAttribute>().First();
                    if (toolboxAttribute.TabName.Contains(filter))
                        yield return moduleType;
                }
            }
        }

        private static AssemblyName LoadAssembly(AssemblyName name){
            if (_loadedAssemblyNames.Contains(name.FullName))
                return null;
            _loadedAssemblyNames.Add(name.FullName);
            Assembly.Load(name);
            return name;
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args){
            var name = args.Name;
            var assemblyName = new AssemblyName(name);
            var path = Path.Combine(_path,assemblyName.Name+".dll");
            return Assembly.LoadFile(path);
        }

        private static void LoadReferences(AssemblyName[] assemblyNames){
            foreach (var assemblyName in assemblyNames.Where(name => name.Name.StartsWith("Xpand"))){
                var assembly = Assembly.Load(assemblyName);
                LoadReferences(assembly.GetReferencedAssemblies());
            }
        }
    }
}
