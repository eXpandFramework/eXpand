using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;

namespace SecurityDemo.Module.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class SecurityDemoAspNetModule : ModuleBase
    {
        public SecurityDemoAspNetModule()
        {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var xpandDLLPath =AppDomain.CurrentDomain.SetupInformation.ApplicationBase+ @"..\..\..\xpand.dll\";
            this.AddModules(xpandDLLPath);

        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
