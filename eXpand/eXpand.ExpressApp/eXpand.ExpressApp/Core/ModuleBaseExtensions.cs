using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.Core
{
    public static class ModuleBaseExtensions
    {
        public static IList<Controller> CollectControllers(this ModuleBase moduleBase)
        {
            return CollectControllers(moduleBase, null);
        }
        public static IList<Controller> CollectControllers(this ModuleBase moduleBase, Predicate<ITypeInfo> filterPredicate)
        {
            ControllersManager controllersManager = moduleBase.ModuleManager.ControllersManager;
            IList<Controller> controllers = filterPredicate != null
                                                ? controllersManager.CollectControllers(filterPredicate)
                                                : controllersManager.CollectControllers(
                                                      info =>
                                                      moduleBase.AssemblyName ==
                                                      ReflectionHelper.GetAssemblyName(info.AssemblyInfo.Assembly));
            return controllers;
        }
    }
}