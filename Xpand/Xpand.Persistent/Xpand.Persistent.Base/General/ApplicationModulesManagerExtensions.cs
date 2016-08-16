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
            var installedModuleTypes = applicationModulesManager.Modules.Select(m => m.GetType()).ToList();
            AddModule(applicationModulesManager, application, moduleBases, installedModuleTypes);
        }

        private static void AddModule(ApplicationModulesManager applicationModulesManager, XafApplication application,
            ModuleBase[] moduleBases, List<Type> installedModuleTypes){
            foreach (var moduleBase in moduleBases){
                if (!installedModuleTypes.Contains(moduleBase.GetType())){
                    installedModuleTypes.Add(moduleBase.GetType());
                    var requiredModuleTypes =
                        moduleBase.RequiredModuleTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>().ToArray();
                    AddModule(applicationModulesManager, application,requiredModuleTypes,installedModuleTypes);
                    applicationModulesManager.AddModule(moduleBase);
                    LoadRegularTypesToTypesInfo(application.TypesInfo, moduleBase);
                    applicationModulesManager.ControllersManager.RegisterControllerTypes(
                        moduleBase.GetControllerTypes().ToArray());
                    
                }
            }
        }
    }
}