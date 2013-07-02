using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.Utils;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace Xpand.ExpressApp.StateMachine {

    [ToolboxBitmap(typeof(XpandStateMachineModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandStateMachineModule : XpandModuleBase {
        public XpandStateMachineModule() {
            RequiredModuleTypes.Add(typeof(StateMachineModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode)
                Application.SetupComplete += ApplicationOnSetupComplete;
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