using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;

namespace SecurityDemo.Module{
    public sealed partial class SecurityDemoModule : ModuleBase{
        static SecurityDemoModule(){
            ObjectMethodActionsViewController.Enabled = false;
        }

        public SecurityDemoModule(){
            InitializeComponent();
        }

        public static string GetXpandDllPath(){
            var xpandDLLPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            while (Directory.GetDirectories(xpandDLLPath).All(s => (new DirectoryInfo(s).Name + "").ToLower() != "xpand.dll")) {
                xpandDLLPath = Path.GetFullPath(xpandDLLPath + @"..\");
            }
            xpandDLLPath += @"Xpand.dll";
            return xpandDLLPath;
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB){
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[]{updater};
        }
    }
}