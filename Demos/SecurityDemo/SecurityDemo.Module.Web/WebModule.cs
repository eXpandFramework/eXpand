using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

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
            var moduleAssembliesPath = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).GetParentFolder("xpand.dll");
            var moduleBases = ModuleActivator.CreateInstances(moduleAssembliesPath, "Web").Distinct();
            foreach (var module in moduleBases) {
                moduleManager.AddModule(Application, module);
            }
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
