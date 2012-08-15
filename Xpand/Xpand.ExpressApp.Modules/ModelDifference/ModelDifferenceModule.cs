using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Builders;
using Xpand.ExpressApp.ModelDifference.NodeUpdaters;
using Xpand.ExpressApp.ModelDifference.Security.Improved;
using Xpand.ExpressApp.SystemModule;


namespace Xpand.ExpressApp.ModelDifference {
    [ToolboxItem(false)]
    public sealed class ModelDifferenceModule : XpandModuleBase {


        public ModelDifferenceModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Security.XpandSecurityModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);

            if (RuntimeMode) {
                var securityComplex = Application.Security as ISecurityComplex;
                if (securityComplex != null) {
                    RoleDifferenceObjectBuilder.CreateDynamicRoleMember(securityComplex);
                }
                if (Application.Security != null)
                    UserDifferenceObjectBuilder.CreateDynamicUserMember(Application.Security.UserType);
            } else {
                CreateDesignTimeCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            }
        }


        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SetupComplete += ApplicationOnSetupComplete;
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = SecuritySystem.Instance as SecurityStrategy;
            if (securityStrategy != null) {
                (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
            }
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