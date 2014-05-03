using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using Fasterflect;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.Xpo;
using PermissionBase = DevExpress.ExpressApp.Security.PermissionBase;

namespace Xpand.Docs.Module.BusinessObjects {
    public static class ParsingExtensions {
        private static ExtendedInterface GetExtendedInterface(IObjectSpace objectSpace,Type extendedInterface) {
            var extendedInterface1 = objectSpace.GetObject<ExtendedInterface>(@interface => @interface.Name == extendedInterface.Name) ;
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
                    var extenderInterface = objectSpace.GetObject<ExtenderInterface>(node => node.Name == extender.Name);
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
            var artifactTypes = moduleType.Assembly.GetTypes().Where(type => !type.IsAbstract).Where(type => moduleArtifactType.IsValidArtifactType(type)).ToArray();
            var objectSpace = moduleChild.XPObjectSpace();
            return artifactTypes.ToList().ToDictionary(type => type, type => CreateArtifact(moduleChild, moduleArtifactType, objectSpace, type));
        }

        private static ModuleArtifact CreateArtifact(ModuleChild moduleChild, ModuleArtifactType moduleArtifactType,IObjectSpace objectSpace, Type type){
            var moduleArtifact = objectSpace.CreateObject<ModuleArtifact>();
            moduleArtifact.Name = type.Name;
            moduleArtifact.Type = moduleArtifactType;
            moduleArtifact.ModuleChilds.Add(moduleChild);
            return moduleArtifact;
        }

        public static ModuleArtifact GetModuleArtifact(this IObjectSpace objectSpace,Type artifactType) {
            var moduleArtifactType = GetModuleArtifactType(artifactType);
            var controllerArtifact = objectSpace.GetObject<ModuleArtifact>(artifact=> artifact.Type == moduleArtifactType && artifact.Name == artifactType.Name);
            return controllerArtifact;
        }

        public static ModuleArtifact CreateArtifact(IObjectSpace objectSpace, Type artifactType){
            var assemblyName = artifactType.Assembly.GetName().Name;
            var moduleChild = objectSpace.FindObject<ModuleChild>(child => child.AssemblyName == assemblyName);
            var moduleArtifactType = GetModuleArtifactType(artifactType);
            return CreateArtifact(moduleChild, moduleArtifactType,objectSpace,  artifactType);
        }

        private static ModuleArtifactType GetModuleArtifactType(Type artifactType){
            if (IsController(artifactType)){
                return ModuleArtifactType.Controller;
            }
            if (IsViewItem(artifactType))
                return ModuleArtifactType.ViewItem;
            throw new NotImplementedException();
        }

        public static bool IsValidArtifactType(this ModuleArtifactType moduleArtifactType, Type type){
            switch (moduleArtifactType) {
                case ModuleArtifactType.ViewItem:
                    return IsViewItem(type);
                case ModuleArtifactType.ListEditor:
                    return IsListEditor(type);
                case ModuleArtifactType.Controller:
                    return IsController(type);
                case ModuleArtifactType.Permission:
                    return IsPermission(type);
                case ModuleArtifactType.BusinessObject:{
                    return IsBusinessObject(type);
                }
            }
            return false;
        }

        private static bool IsBusinessObject(Type type){
            var isValidBOType =
                !new[]{typeof (XpandPermissionData), typeof (PermissionBase)}.Any(type1 => type1.IsAssignableFrom(type));
            return type.IsDomainComponent() && isValidBOType;
        }

        private static bool IsPermission(Type type){
            return typeof(IOperationPermission).IsAssignableFrom(type);
        }

        private static bool IsListEditor(Type type){
            return typeof(ListEditor).IsAssignableFrom(type);
        }

        private static bool IsViewItem(Type type){
            return typeof(ViewItem).IsAssignableFrom(type);
        }

        private static bool IsController(Type type){
            return typeof(Controller).IsAssignableFrom(type);
        }

        public static void CreateArtifacts(this ModuleChild moduleChild, Type moduleType) {
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.ViewItem);
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.ListEditor);
            moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.Permission);
            var controllers = moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.Controller);
            InterfaceBuilder.SkipAssemblyCleanup = true;
            foreach (var pair in controllers){
                var extenderInterfaces = moduleChild.CreateExtenderInterfaces(pair.Key);
                pair.Value.Artifacts.AddRange(extenderInterfaces);
                moduleChild.CreateArtifacts(moduleType, ModuleArtifactType.Action);
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

        public static Dictionary<Type, ModuleChild> GetModuleChilds(this IObjectSpace objectSpace, IEnumerable<Type> moduleTypes) {
            var moduleChildren = new Dictionary<Type, ModuleChild>();
            foreach (var moduleType in moduleTypes) {
                var assemblyName = moduleType.Assembly.GetName().Name;
                var name = moduleType.Name;
                var moduleChild =objectSpace.GetObject<ModuleChild>(child => child.AssemblyName==assemblyName&&child.Name==name);
                moduleChild.AssemblyName = assemblyName;
                moduleChild.Name = name;
                moduleChild.Platform = GetPlatform(moduleChild.AssemblyName);
                moduleChildren.Add(moduleType, moduleChild);
            }
            return moduleChildren;
        }
    }
}
