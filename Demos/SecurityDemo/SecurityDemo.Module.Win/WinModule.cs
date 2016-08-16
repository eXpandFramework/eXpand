using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using FeatureCenter.Module.Win;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace SecurityDemo.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class SecurityDemoWindowsFormsModule : ModuleBase
    {
        public SecurityDemoWindowsFormsModule()
        {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            var moduleAssembliesPath = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).GetParentFolder("xpand.dll");
            var moduleBases = ModuleActivator.CreateInstances(moduleAssembliesPath, "Win").Distinct();
            foreach (var module in moduleBases){
                moduleManager.AddModule(Application, module);
            }
        }


        public override ICollection<Type> GetXafResourceLocalizerTypes() {
            ICollection<Type> result = new List<Type>();
            result.Add(typeof(FeatureCenterMainFormTemplateLocalizer));
            result.Add(typeof(FeatureCenterPopupFormTemplateLocalizer));
            return result;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
