using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using Fasterflect;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.Utils.Linq;

namespace Xpand.Test.AutoFixture.Customizations{
    public class ModuleBaseCustomization:ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new ModuleBaseSpecimentBuilder());
        }
    }


    public class ModuleBaseCustomization<TModule> : ICustomization where TModule : ModuleBase {
        public void Customize(IFixture fixture) {
            fixture.FreezeModuleBase<TModule>();
        }
    }

    public class ModuleBaseSpecimentBuilder :  ISpecimenBuilder{
        
        public object Create(object request, ISpecimenContext context){
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo != null&&typeof(ModuleBase).IsAssignableFrom(parameterInfo.ParameterType)){
                var parameterType = parameterInfo.ParameterType;
                object moduleBase = context.Resolve(parameterType);
                Setup((ModuleBase) moduleBase);
                return moduleBase;
            }
            return new NoSpecimen();
        }

        public static void Setup(ModuleBase moduleBase){
            var moduleBases =
                moduleBase.RequiredModuleTypes.GetItems<Type>(type => ((ModuleBase) type.CreateInstance()).RequiredModuleTypes)
                    .Select(type => type.CreateInstance()).Cast<ModuleBase>().Distinct()
                    .ToArray();
            foreach (var module in moduleBases){
                if (!moduleBase.Application.Modules.Select(appModule => appModule.GetType()).Contains(module.GetType()))
                    moduleBase.Application.Modules.Add(module);
            }
            moduleBase.Application.Modules.Add(moduleBase);

            moduleBase.Application.Setup();
        }
    }
}