using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils.CodeGeneration;
using DevExpress.ExpressApp.Utils.Reflection;
using Fasterflect;
using HarmonyLib;

namespace Xpand.Persistent.Base.General{
    public static class ApplicationModulesManagerExtensions {
        static ApplicationModulesManagerExtensions() {
            var harmony = new Harmony(typeof(ApplicationModulesManagerExtensions).Namespace);
            var prefix = typeof(ApplicationModulesManagerExtensions).Method(nameof(ModifyCSCodeCompilerReferences),Flags.Static|Flags.AnyVisibility);
            var original = typeof(CSCodeCompiler).GetMethod(nameof(CSCodeCompiler.Compile));
            harmony.Patch(original, new HarmonyMethod(prefix));
        }
        public static void AddModelReferences(this ApplicationModulesManager _, params string[] references) {
            foreach (var reference in references) {
                References.Add(reference);
            }
        }
        static readonly ConcurrentBag<string> References=new ConcurrentBag<string>();
        internal static void ModifyCSCodeCompilerReferences(string sourceCode, ref string[] references, string assemblyFile){
            references = references.Concat(References).ToArray();
        }
        private static void LoadRegularTypesToTypesInfo(ITypesInfo typesInfo,ModuleBase module) {
            IEnumerable<Type> regularTypes = AssemblyHelper.GetTypesFromAssembly(module.GetType().Assembly);
            if (regularTypes != null) {
                foreach (Type regularType in regularTypes) {
                    typesInfo.FindTypeInfo(regularType);
                }
            }
            ((DcAssemblyInfo) XafTypesInfo.Instance.FindAssemblyInfo(module.GetType().Assembly)).AllTypesLoaded = true;
        }

        public static void AddModule(this ApplicationModulesManager applicationModulesManager, XafApplication application,params ModuleBase[] moduleBases){
            var moduleTypes = new HashSet<string>(applicationModulesManager.Modules.Select(m => m.Name));
            AddModule(applicationModulesManager, application, moduleBases, moduleTypes);
        }

        private static void AddModule(ApplicationModulesManager applicationModulesManager, XafApplication application,
            ModuleBase[] moduleBases, HashSet<string> installedModules){
            foreach (var moduleBase in moduleBases){
                if (!installedModules.Contains(moduleBase.Name)){
                    installedModules.Add(moduleBase.Name);
                    var requiredModuleTypes = moduleBase.RequiredModuleTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>().ToArray();
                    AddModule(applicationModulesManager, application,requiredModuleTypes,installedModules);
                    applicationModulesManager.AddModule(moduleBase);
                    moduleBase.Application=application;
                    LoadRegularTypesToTypesInfo(XafTypesInfo.Instance, moduleBase);
                    applicationModulesManager.ControllersManager.RegisterControllerTypes(moduleBase.GetControllerTypes().ToArray());
                    
                }
            }
        }
    }
}