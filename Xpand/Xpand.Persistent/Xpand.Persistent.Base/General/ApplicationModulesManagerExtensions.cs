using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Fasterflect;

namespace Xpand.Persistent.Base.General{
    public static class ApplicationModulesManagerExtensions {
        public static void AddModule(this ApplicationModulesManager applicationModulesManager, XafApplication application,params ModuleBase[] moduleBases) {
            var relatedControllerTypes=new List<Type>();
            foreach (var moduleBase in moduleBases){
                applicationModulesManager.AddModule(moduleBase);
                moduleBase.Setup(application);
                var controllerTypes = GetControllerTypes(moduleBase);
                relatedControllerTypes.AddRange(controllerTypes);
            }
            applicationModulesManager.ControllersManager.RegisterControllerTypes(relatedControllerTypes.ToArray());
            
        }

        private static IEnumerable<Type> GetControllerTypes(ModuleBase moduleBase){
            foreach (var controllerType in moduleBase.GetControllerTypes()){
                yield return controllerType;
            }
            var requiredModules = moduleBase.RequiredModuleTypes.Select(type => type.CreateInstance()).Cast<ModuleBase>();
            
            foreach (var module in requiredModules){
                module.Application = moduleBase.Application;
                foreach (var type in module.GetControllerTypes()){
                    yield return type;
                }
            }
        }
    }
}