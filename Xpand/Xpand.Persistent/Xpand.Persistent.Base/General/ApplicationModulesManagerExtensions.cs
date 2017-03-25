using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils.Reflection;
using Fasterflect;

namespace Xpand.Persistent.Base.General{
    public static class ApplicationModulesManagerExtensions {
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