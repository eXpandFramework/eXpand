using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.Docs.Module.BusinessObjects;
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
            var moduleChild = ObjectSpace.FindObject<ModuleChild>(null);
            
            if (moduleChild==null){
                var moduleTypes = GetModuleTypes();
                var childModules = ObjectSpace.CreateModuleChilds(moduleTypes);
                foreach (var childModule in childModules){
                    childModule.Value.CreateArtifacts(childModule.Key);
                    childModule.Value.CreateExtenderInterfaces(childModule.Key);
                }
                CreateSecurityObjects();
                UpdateMapViewModule(childModules);
                ObjectSpace.CommitChanges();
                throw new NotImplementedException("Please restart");
            }
        }

        private void UpdateMapViewModule(Dictionary<Type, ModuleChild> childModules){
            var modules = ObjectSpace.CreateModules(childModules.Values).ToList();
            var mapViewModule = modules.First(module => module.Name == "MapView");
            mapViewModule.Installation =GetType().Assembly.GetManifestResourceStream(GetType(), "MapViewInstalation.xml").ReadToEndAsString();
            var document = ObjectSpace.CreateObject<Document>();
            document.Name = "How to display an address from and object property in Google Maps";
            document.Text =GetType().Assembly.GetManifestResourceStream(GetType(), "HowToDisplayAddressInGoogleMaps.xml").ReadToEndAsString();
            document.Author = ObjectSpace.CreateObject<Author>();
            document.Author.FirstName = "Apostolis";
            document.Author.FirstName = "Bekiaris";
            mapViewModule.Documents.Add(document);
        }

        private void CreateSecurityObjects(){
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            var adminRole = ObjectSpace.GetAdminRole("Admin");
            var adminUser = (XpandUser)adminRole.GetUser("Admin");
            adminUser.Email = "apostolis.bekiaris@gmail.com";

            var anonymousRole = ObjectSpace.GetAnonymousRole("Anonymous");
            anonymousRole.GetAnonymousUser();

            var userRole = (XpandRole) ObjectSpace.GetRole("User");
            var typeInfos = XafTypesInfo.Instance.PersistentTypes.Where(info => typeof(DocsBaseObject).IsAssignableFrom(info.Type));
            foreach (var typeInfo in typeInfos){
                userRole.EnsureTypePermissions(typeInfo.Type,SecurityOperations.ReadOnlyAccess);
                userRole.Permissions.Add(IOActionPermission());
                anonymousRole.Permissions.Add(IOActionPermission());
                anonymousRole.EnsureTypePermissions(typeInfo.Type, SecurityOperations.ReadOnlyAccess);
                if (typeof (ModuleArtifact).IsAssignableFrom(typeInfo.Type))
                    userRole.AddMemberAccessPermission(typeInfo.Type, "Text,Author,Url", SecurityOperations.Write,"Creator=CurrentUserId() or Text Is  Null");
            }
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);
        }

        private ActionStateOperationPermissionData IOActionPermission(){
            var actionStateOperationPermissionData = ObjectSpace.CreateObject<ActionStateOperationPermissionData>();
            actionStateOperationPermissionData.ActionId = "IO";
            actionStateOperationPermissionData.ActionState=ActionState.Hidden;
            actionStateOperationPermissionData.ID = "Hide IO Action";
            return actionStateOperationPermissionData;
        }


        private IEnumerable<Type> GetModuleTypes() {
            var fullPath = Path.GetFullPath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"..\..\Xpand.dll");
            if (!XpandModuleBase.IsHosted)
                fullPath = Path.GetFullPath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"..\..\..\..\Xpand.dll");
//            var files = Directory.GetFiles(fullPath, "Xpand.ExpressApp*.dll", SearchOption.TopDirectoryOnly).Where(s => s.Contains("ModelDiff"));
            var files = Directory.GetFiles(fullPath, "Xpand.ExpressApp*.dll", SearchOption.TopDirectoryOnly);
            return files.Select(Assembly.LoadFrom).Select(assembly => assembly.GetTypes().FirstOrDefault(type => !type.IsAbstract && typeof(XpandModuleBase).IsAssignableFrom(type))).Where(moduleType => moduleType != null);
        }
    }
}
