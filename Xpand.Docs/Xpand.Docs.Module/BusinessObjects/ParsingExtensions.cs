using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Fasterflect;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.Xpo;
using PermissionBase = DevExpress.ExpressApp.Security.PermissionBase;

namespace Xpand.Docs.Module.BusinessObjects {
    public static class ParsingExtensions {
        private static ExtendedInterface GetExtendedInterface(IObjectSpace objectSpace,Type extendedInterface) {
            var extendedInterface1 = objectSpace.FindObject<ExtendedInterface>(@interface => @interface.Name == extendedInterface.Name, PersistentCriteriaEvaluationBehavior.InTransaction) ?? objectSpace.CreateObject<ExtendedInterface>();
            extendedInterface1.Name = extendedInterface.Name;
            return extendedInterface1;
        }

        private static ModelInterfaceExtenders GetModelInterfaceExtenders(Type moduleType) {            
            var modelInterfaceExtenders = new ModelInterfaceExtenders();
            if (typeof(XpandModuleBase).IsAssignableFrom(moduleType)){
                XpandModuleBase.CallMonitor.Clear();
                ((XpandModuleBase)moduleType.CreateInstance()).ExtendModelInterfaces(modelInterfaceExtenders);
            }
            if (typeof(IModelExtender).IsAssignableFrom(moduleType))
                ((IModelExtender) moduleType.CreateInstance()).ExtendModelInterfaces(modelInterfaceExtenders);
            return modelInterfaceExtenders;
        }

        private static IEnumerable<ModelNodeAttribute> CreateModelNodeAttributes(IObjectSpace objectSpace,Type interfaceExtender) {
            foreach (var propertyInfo in interfaceExtender.Properties()) {
                var modelNodeAttribute = objectSpace.CreateObject<ModelNodeAttribute>();
                modelNodeAttribute.Name = propertyInfo.Name;
                var descriptionAttribute = propertyInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().FirstOrDefault();
                if (descriptionAttribute != null)
                    modelNodeAttribute.Text = descriptionAttribute.Description;
                yield return modelNodeAttribute;
            }
        }

        public static IList<ExtenderInterface> CreateExtenderInterfaces(this ModuleChild moduleChild,Type artifactType) {
            var modelInterfaceExtenders = GetModelInterfaceExtenders(artifactType);
            var objectSpace = moduleChild.XPObjectSpace();
            var extenderInterfaces = new List<ExtenderInterface>();
            foreach (var extendedInterface in modelInterfaceExtenders.GetExtendedInterfaces().Where(type => !type.Name.StartsWith("IModelModelAdaptor"))) {
                var interfaceExtenders = modelInterfaceExtenders.GetInterfaceExtenders(extendedInterface);
                foreach (var interfaceExtender in interfaceExtenders) {
                    Type extender = interfaceExtender;
                    var extenderInterface = objectSpace.FindObject<ExtenderInterface>(node => node.Name == extender.Name,
                            PersistentCriteriaEvaluationBehavior.InTransaction) ?? objectSpace.CreateObject<ExtenderInterface>();
                    if (objectSpace.IsNewObject(extenderInterface)) {
                        extenderInterface.Name = interfaceExtender.Name;
                        extenderInterface.ExtendedInterface = GetExtendedInterface(objectSpace, extendedInterface);
                    }
                    extenderInterface.ModuleChilds.Add(moduleChild);
                    var modelNodeAttributes = CreateModelNodeAttributes(objectSpace, interfaceExtender);
                    extenderInterface.Attributes.AddRange(modelNodeAttributes);
                    extenderInterfaces.Add(extenderInterface);
                }
            }
            return extenderInterfaces;
        }

        public static Dictionary<Type, ModuleArtifact> CreateArtifacts(this ModuleChild moduleChild, Type moduleType, ModuleArtifactType moduleArtifactType) {
            var artifactTypes = moduleType.Assembly.GetTypes().Where(type => !type.IsAbstract).Where(type => IsValidArtifactType(moduleArtifactType, type)).ToArray();
            var moduleArtifacts = new Dictionary<Type, ModuleArtifact>();
            var objectSpace = moduleChild.XPObjectSpace();
            foreach (var type in artifactTypes) {
                var moduleArtifact = objectSpace.CreateObject<ModuleArtifact>();
                moduleArtifact.Name = type.Name;
                moduleArtifact.Type = moduleArtifactType;
                moduleArtifacts.Add(type, moduleArtifact);
                moduleArtifact.ModuleChilds.Add(moduleChild);
            }
            return moduleArtifacts;
        }

        private static bool IsValidArtifactType(ModuleArtifactType moduleArtifactType, Type type){
            switch (moduleArtifactType) {
                case ModuleArtifactType.ViewItem:
                    return typeof(ViewItem).IsAssignableFrom(type);
                case ModuleArtifactType.ListEditor:
                    return typeof(ListEditor).IsAssignableFrom(type);
                case ModuleArtifactType.Controller:
                    return typeof(Controller).IsAssignableFrom(type);
                case ModuleArtifactType.Permission:
                    return typeof(IOperationPermission).IsAssignableFrom(type);
                case ModuleArtifactType.BusinessObject:{
                    var isValidBOType = !new[] { typeof(XpandPermissionData), typeof(PermissionBase) }.Any(type1 => type1.IsAssignableFrom(type));
                    return type.IsDomainComponent() && isValidBOType;
                }
            }
            return false;
        }

        public static void CreateArtifacts(this ModuleChild moduleChild, Type moduleType) {
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.ViewItem);
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.ListEditor);
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.Permission);
            var moduleArtifacts = moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.Controller);
            InterfaceBuilder.SkipAssemblyCleanup = true;
            foreach (var moduleArtifact in moduleArtifacts){
                var extenderInterfaces = moduleChild.CreateExtenderInterfaces(moduleArtifact.Key);
                moduleArtifact.Value.Artifacts.AddRange(extenderInterfaces);
            }
            InterfaceBuilder.SkipAssemblyCleanup = false;
            moduleChild.CreateArtifacts( moduleType, ModuleArtifactType.BusinessObject);
        }

        private static string GroupedModuleName(ModuleChild moduleChild) {
            var groupedModuleName = Regex.Match(moduleChild.AssemblyName, "Xpand.ExpressApp.([^.]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
            return new[] { "", "Web", "Win" }.Contains(groupedModuleName) ? "Core" : groupedModuleName;
        }

        public static IEnumerable<Module> CreateModules(this IObjectSpace objectSpace, IEnumerable<ModuleChild> moduleChildren) {
            var groupBy = moduleChildren.GroupBy(GroupedModuleName);
            foreach (var group in groupBy) {
                var o = objectSpace.CreateObject<Module>();
                o.Name = @group.Key;
                o.ModuleChilds.AddRange(group);
                yield return o;
            }
        }
        private static Platform GetPlatform(string assemblyName) {
            return assemblyName.EndsWith(".Win")
                ? Platform.Win
                : (assemblyName.EndsWith(".Web") ? Platform.Web : Platform.Both);
        }

        public static Dictionary<Type, ModuleChild> CreateModuleChilds(this IObjectSpace objectSpace, IEnumerable<Type> moduleTypes) {
            var moduleChildren = new Dictionary<Type, ModuleChild>();
            foreach (var moduleType in moduleTypes) {
                var moduleChild = objectSpace.CreateObject<ModuleChild>();
                moduleChild.AssemblyName = moduleType.Assembly.GetName().Name;
                moduleChild.Name = moduleType.Name;
                moduleChild.Platform = GetPlatform(moduleChild.AssemblyName);
                moduleChildren.Add(moduleType, moduleChild);
            }
            return moduleChildren;
        }
    }
}
