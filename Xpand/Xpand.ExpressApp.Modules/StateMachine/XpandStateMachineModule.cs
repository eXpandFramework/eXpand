using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.Validation;
using DevExpress.Utils;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.StateMachine {

    [ToolboxBitmap(typeof(XpandStateMachineModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandStateMachineModule : XpandModuleBase {
        public const string AdminRoles = "AdminRoles";
        public const string EnableFilteredPropety = "EnableFilteredPropety";
        public XpandStateMachineModule() {
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(StateMachineModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode)
                Application.SetupComplete += ApplicationOnSetupComplete;
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SettingUp += ApplicationOnSettingUp;
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo<XpoStateMachine>();
            typeInfo.CreateMember(EnableFilteredPropety, typeof (bool));
            if (!RuntimeMode) {
                CreateDesignTimeCollection(typesInfo, typeof(XpoStateMachine), AdminRoles);
            } else if (Application.CanBuildSecurityObjects()) {
                BuildSecuritySystemObjects();
            }
        }

        void ApplicationOnSettingUp(object sender, EventArgs eventArgs) {
            BuildSecuritySystemObjects();
        }

        void BuildSecuritySystemObjects() {
            var dynamicSecuritySystemObjects = new DynamicSecuritySystemObjects(Application);
            var xpMemberInfos = dynamicSecuritySystemObjects.BuildRole(typeof(XpoStateMachine), "StateMachineRoles", "XpoStateMachines", AdminRoles);
            dynamicSecuritySystemObjects.HideInDetailView(xpMemberInfos, "XpoStateMachines");
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = ((XafApplication)sender).Security as SecurityStrategy;
            if (securityStrategy != null) (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            customizeRequestProcessorsEventArgs.Processors.Add(new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(StateMachineTransitionOperationRequest), new StateMachineTransitionRequestProcessor(customizeRequestProcessorsEventArgs.Permissions)));
        }

    }
}