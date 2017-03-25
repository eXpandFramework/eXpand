using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Fasterflect;
using Mono.Cecil;
using Xpand.Utils.Linq;

namespace Xpand.Persistent.Base.General {
    public static class ModuleActivator {
        // ReSharper disable once UnusedMember.Local
        private static readonly Destructor _finalise = new Destructor();
        private sealed class Destructor {
            ~Destructor() {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            }
        }
        private static string _path;

        static ModuleActivator(){
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }
        public static void AddModules(this ModuleBase moduleBase, string xpandDLLPath){
            var filter = moduleBase.ModuleManager.Modules.AreHosted() ? XpandAssemblyInfo.TabAspNetModules: XpandAssemblyInfo.TabWinModules;
            var moduleBases = CreateInstances(xpandDLLPath, filter).OrderBy(m => m.GetType().FullName).ToArray();
            ModuleBase first = null;
//            first = moduleBases[35];
            foreach (var module in moduleBases.Where(m => m!=first)) {
                moduleBase.ModuleManager.AddModule(moduleBase.Application, module);
            }
        }

        public static IEnumerable<ModuleBase> CreateInstances(string path,string tabNameFilter,string searchPattern="Xpand.ExpressApp*.dll"){
            _path = Path.GetFullPath(path);
            
            var filteredTypes = GetModuleTypes(tabNameFilter,searchPattern);
            var filteredInstances =filteredTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>().ToArray();
            var requiredModuleTypes = filteredInstances.GetItems<ModuleBase>(m => m.RequiredModuleTypes).Select(m => m.GetType()).Distinct();
            var agnosticTypes =GetModuleTypes("Win-Web",searchPattern).Except(requiredModuleTypes);
            filteredInstances= filteredInstances.Concat(agnosticTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>()).ToArray();
            return filteredInstances.DistinctBy(m => m.GetType());
        }


        private static IEnumerable<Type> GetModuleTypes(string filter,string searchPattern){
            foreach (var file in Directory.GetFiles(_path, searchPattern)){
                var assemblyDefinition = AssemblyDefinition.ReadAssembly(file);
                var typeDefinition = assemblyDefinition.MainModule.Types.FirstOrDefault(definition => definition.CustomAttributes.Any(attribute => attribute.AttributeType.Name==typeof(ToolboxTabNameAttribute).Name));
                if (typeDefinition != null){
                    var toolboxAttribute = typeDefinition.CustomAttributes.First(attribute => attribute.AttributeType.Name==typeof(ToolboxTabNameAttribute).Name);
                    var argument = toolboxAttribute.ConstructorArguments[0].Value.ToString();
                    if (argument.Contains(filter)){
                        yield return Assembly.Load(file).GetTypes().First(type => type.GetCustomAttributes(typeof(ToolboxTabNameAttribute),false).Any());
                    }
                }
            }
        }

        static readonly Dictionary<string,Assembly> _loadedAssemblies=new Dictionary<string,Assembly>();
        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args){
            var name = args.Name;
            var fileName = Path.GetFileName(args.Name) + "";
            if (name.Contains(","))
                fileName = name.Substring(0,name.IndexOf(",", StringComparison.Ordinal)) + ".dll";
            
            var path = Path.Combine(_path,fileName);
            if (File.Exists(path)){
                if (!_loadedAssemblies.ContainsKey(fileName))
                    _loadedAssemblies.Add(fileName, Assembly.LoadFile(path));
                return _loadedAssemblies[fileName];
            }
            return null;
        }

    }
}
