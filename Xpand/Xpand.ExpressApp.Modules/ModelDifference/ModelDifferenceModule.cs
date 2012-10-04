using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.NodeUpdaters;
using Xpand.ExpressApp.ModelDifference.Security.Improved;


namespace Xpand.ExpressApp.ModelDifference {
    [ToolboxItem(false)]
    public sealed class ModelDifferenceModule : XpandModuleBase {
        public ModelDifferenceModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Security.XpandSecurityModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!RuntimeMode) {
                CreateDesignTimeCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            } else if ((Application.Security.UserType != null && !Application.Security.UserType.IsInterface)) {
                BuildSecuritySystemObjects();
            }
        }


        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SettingUp += ApplicationOnSetupComplete;
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            BuildSecuritySystemObjects();
            var securityStrategy = SecuritySystem.Instance as SecurityStrategy;
            if (securityStrategy != null) {
                (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
            }
        }

        void BuildSecuritySystemObjects() {
            var dynamicSecuritySystemObjects = new DynamicSecuritySystemObjects(Application);
            dynamicSecuritySystemObjects.BuildUser(typeof(UserModelDifferenceObject), "UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects", "UserModelDifferenceObjects", "Users");
            dynamicSecuritySystemObjects.BuildRole(typeof(RoleModelDifferenceObject), "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModelDifferenceObjects", "Roles");
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            var modelCombineRequestProcessor = new ModelCombineRequestProcessor(customizeRequestProcessorsEventArgs.Permissions);
            var keyValuePair = new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(ModelCombinePermissionRequest), modelCombineRequestProcessor);
            customizeRequestProcessorsEventArgs.Processors.Add(keyValuePair);
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new BOModelNodesUpdater());
            updaters.Add(new BOModelMemberNodesUpdater());
        }
    }
}