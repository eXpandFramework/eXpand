using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Docs.Module.BusinessObjects;
using Xpand.ExpressApp.AuditTrail;
using Xpand.ExpressApp.AuditTrail.BusinessObjects;
using Xpand.ExpressApp.Email.BusinessObjects;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Security.Improved;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;


namespace Xpand.Docs.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }


        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            ParseXpandModules();
            CreateSecurityObjects();
            CreateRegistrationEmailTemplates(ObjectSpace);

            var version = new Version(XpandAssemblyInfo.Version);
            if (version<new Version(13,2,9,28)){
                var userRole = (XpandRole) ObjectSpace.GetRole("User");
                userRole.SetTypePermissions<XpandAuditDataItemPersistent>(SecurityOperations.ReadOnlyAccess,SecuritySystemModifier.Allow);
                ApproveAuditsActionPermission(userRole);
                userRole.SetTypePermissions<Document>(SecurityOperations.Create,SecuritySystemModifier.Allow);
                userRole.AddMemberAccessPermission<Document>("Name,Url,Author", SecurityOperations.ReadWriteAccess,"Creator Is Null");
            }
            
        }

        private void ApproveAuditsActionPermission(XpandRole userRole){
            var actionStateRulePermission = ObjectSpace.CreateObject<ActionStateOperationPermissionData>();
            actionStateRulePermission.ObjectTypeData = typeof (object);
            actionStateRulePermission.ActionId = AuditPendingController.ApproveAudits;
            actionStateRulePermission.ActionState = ActionState.Hidden;
            actionStateRulePermission.ID = "Hide ApproveAudits";
            userRole.Permissions.Add(actionStateRulePermission);
        }

        private void CreateRegistrationEmailTemplates(IObjectSpace objectSpace) {
            if (!objectSpace.Contains<EmailTemplate>()) {
                var emailTemplate = objectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Configure(EmailTemplateConfig.UserActivation, "http://localhost:50822/");

                emailTemplate = objectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Configure(EmailTemplateConfig.PassForgotten);
            }
        }


        private void CreateSecurityObjects() {
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole)){
                var adminRole = ObjectSpace.GetAdminRole("Admin");
                var adminUser = (XpandUser)adminRole.GetUser("Admin", ConfigurationManager.AppSettings["AdminDefaultPass"]);
                adminUser.Email = "apostolis.bekiaris@gmail.com";

                var anonymousRole = ObjectSpace.GetAnonymousRole("Anonymous");
                anonymousRole.GetAnonymousUser();

                var userRole = (XpandRole)ObjectSpace.GetRole("User");
                var typeInfos = XafTypesInfo.Instance.PersistentTypes.Where(info => typeof(DocsBaseObject).IsAssignableFrom(info.Type));
                foreach (var typeInfo in typeInfos) {
                    userRole.EnsureTypePermissions(typeInfo.Type, SecurityOperations.ReadOnlyAccess);
                    userRole.Permissions.Add(IOActionPermission());
                    anonymousRole.Permissions.Add(IOActionPermission());
                    anonymousRole.EnsureTypePermissions(typeInfo.Type, SecurityOperations.ReadOnlyAccess);
                    if (typeof(ModuleArtifact).IsAssignableFrom(typeInfo.Type))
                        userRole.AddMemberAccessPermission(typeInfo.Type, "Text,Author,Url", SecurityOperations.Write, "Creator=CurrentUserId() or Text Is  Null");
                }
                var user = (SecuritySystemUser)userRole.GetUser("user");
                user.Roles.Add(defaultRole);
            }
        }

        private ActionStateOperationPermissionData IOActionPermission() {
            var actionStateOperationPermissionData = ObjectSpace.CreateObject<ActionStateOperationPermissionData>();
            actionStateOperationPermissionData.ActionId = "IO";
            actionStateOperationPermissionData.ActionState = ActionState.Hidden;
            actionStateOperationPermissionData.ID = "Hide IO Action";
            return actionStateOperationPermissionData;
        }

        private void ParseXpandModules() {
            var domainSetup = new AppDomainSetup{
                ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath
            };

            AppDomain childDomain = AppDomain.CreateDomain("Temp", null, domainSetup);
            var runtime = (IRuntime) childDomain.CreateInstanceAndUnwrap(typeof (Runtime).Assembly.FullName, typeof (Runtime).FullName);
            var connectionString = XpandModuleBase.ConnectionString;
            runtime.Run(new RuntimeSetupInfo{XpandRootPath = this.XpandRootPath(),ConnectionString=connectionString});
            AppDomain.Unload(childDomain);
        }
    }

    public interface IRuntime {
        bool Run(RuntimeSetupInfo setupInfo);
    }

    [Serializable]
    public class RuntimeSetupInfo{
        public string XpandRootPath { get; set; }

        public string ConnectionString { get; set; }
    }

    public class Runtime : MarshalByRefObject, IRuntime {
        public bool Run(RuntimeSetupInfo setupInfo) {
            var typesInfo = new TypesInfo();
            typesInfo.AddEntityStore(new NonPersistentEntityStore(typesInfo));
            var reflectionDictionary = new ReflectionDictionary();reflectionDictionary.CollectClassInfos(typeof(ModuleArtifact).Assembly);
            var xpoTypeInfoSource = new XpoTypeInfoSource(typesInfo,reflectionDictionary);
            typesInfo.AddEntityStore(xpoTypeInfoSource);
            typesInfo.LoadTypes(typeof(ModuleArtifact).Assembly);
            var exportedTypesFromAssembly = ModuleHelper.CollectExportedTypesFromAssembly(typeof(ModuleArtifact).Assembly,ExportedTypeHelpers.IsExportedType);
            foreach (var type in exportedTypesFromAssembly){
                xpoTypeInfoSource.RegisterEntity(type);
            }

            var objectSpace = new XPObjectSpace(typesInfo, xpoTypeInfoSource, () => new UnitOfWork(reflectionDictionary){ ConnectionString = setupInfo.ConnectionString });
            if (!objectSpace.Contains<ModuleChild>()){
                var moduleTypes = GetModuleTypes(setupInfo);
                var childModules = objectSpace.GetModuleChilds(moduleTypes);
                foreach (var childModule in childModules){
                    childModule.Value.CreateArtifacts(childModule.Key);
                    childModule.Value.CreateExtenderInterfaces(childModule.Key);
                }
                UpdateMapViewModule(childModules, objectSpace);
            }
            CreateObjects(objectSpace, setupInfo);
            objectSpace.CommitChanges();
            return true;
        }

        private static IEnumerable<Type> GetModuleTypes(RuntimeSetupInfo setupInfo){
            var fullPath = setupInfo.XpandRootPath + @"\Xpand.DLL";
            //            var files = Directory.GetFiles(fullPath, "Xpand.ExpressApp*.dll", SearchOption.TopDirectoryOnly).Where(s => s.Contains("ModelDiff"));
            var files = Directory.GetFiles(fullPath, "Xpand.ExpressApp*.dll", SearchOption.TopDirectoryOnly);
            return files.Select(Assembly.LoadFrom).Select(assembly =>assembly.GetTypes().FirstOrDefault(type => !type.IsAbstract && typeof (XpandModuleBase).IsAssignableFrom(type))).Where(moduleType => moduleType != null);
        }

        private void CreateObjects(IObjectSpace objectSpace, RuntimeSetupInfo setupInfo) {
            CreateActionArtifacsts(objectSpace,setupInfo);
        }

        private void CreateActionArtifacsts(IObjectSpace objectSpace, RuntimeSetupInfo setupInfo) {
            if (!objectSpace.Contains<ModuleArtifact>(artifact => artifact.Type == ModuleArtifactType.Action)) {
                var moduleTypes = GetModuleTypes(setupInfo);
                var controllers = moduleTypes.SelectMany(type => type.Assembly.GetTypes())
                        .Where(type => !type.IsAbstract && typeof(Controller).IsAssignableFrom(type))
                        .Select(type => type.CreateInstance())
                        .Cast<Controller>();
                foreach (var controller in controllers) {
                    var controllerArtifact = objectSpace.GetModuleArtifact(controller.GetType());
                    foreach (var action in controller.Actions) {
                        var name = action.Id;
                        var actionArtifact = objectSpace.GetObject<ModuleArtifact>(artifact => artifact.Name == name && artifact.Type == ModuleArtifactType.Action);
                        actionArtifact.Name = name;
                        actionArtifact.Type = ModuleArtifactType.Action;
                        actionArtifact.Text = action.ToolTip;
                        actionArtifact.ModuleChilds.AddRange(controllerArtifact.ModuleChilds);
                        actionArtifact.Artifacts.Add(controllerArtifact);
                        controllerArtifact.Artifacts.Add(actionArtifact);
                    }
                }
            }
        }


        private void UpdateMapViewModule(Dictionary<Type, ModuleChild> childModules,IObjectSpace objectSpace) {
            var modules = objectSpace.CreateModules(childModules.Values).ToList();
            var mapViewModule = modules.First(module => module.Name == "MapView");
            mapViewModule.Installation = GetType().Assembly.GetManifestResourceStream(GetType(), "MapViewInstalation.xml").ReadToEndAsString();
            var document = objectSpace.CreateObject<Document>();
            document.Name = "How to display an address from and object property in Google Maps";
            document.Text = GetType().Assembly.GetManifestResourceStream(GetType(), "HowToDisplayAddressInGoogleMaps.xml").ReadToEndAsString();
            document.Author = "Apostolis Bekiaris";
            mapViewModule.Documents.Add(document);
        }

    }
}
