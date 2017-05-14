using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using FeatureCenter.Module.Win;
using Xpand.Persistent.Base.General;

namespace SecurityDemo.Module.Win{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class SecurityDemoWindowsFormsModule : ModuleBase{
        public SecurityDemoWindowsFormsModule(){
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            var directoryName = Path.GetDirectoryName(new Uri(GetType().Assembly.Location + "").LocalPath);
            var xpandDLLPath = SecurityDemoModule.GetXpandDllPath(directoryName);
            this.AddModules(xpandDLLPath);
        }


        public override ICollection<Type> GetXafResourceLocalizerTypes(){
            ICollection<Type> result = new List<Type>();
            result.Add(typeof(FeatureCenterMainFormTemplateLocalizer));
            result.Add(typeof(FeatureCenterPopupFormTemplateLocalizer));
            return result;
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB){
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}