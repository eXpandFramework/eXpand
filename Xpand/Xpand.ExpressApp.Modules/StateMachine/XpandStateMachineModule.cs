using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace Xpand.ExpressApp.StateMachine {

    [ToolboxBitmap(typeof(XpandStateMachineModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandStateMachineModule : XpandModuleBase {
        public XpandStateMachineModule() {
            InitializeComponent();
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode)
                Application.SetupComplete += ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            if (SecuritySystem.Instance is SecurityStrategy)
                ((SecurityStrategy)SecuritySystem.Instance).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            customizeRequestProcessorsEventArgs.Processors.Add(new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(StateMachineTransitionRequestProcessor), new StateMachineTransitionRequestProcessor(customizeRequestProcessorsEventArgs.Permissions)));
        }

    }
}