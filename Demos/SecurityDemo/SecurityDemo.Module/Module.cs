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

        public static string GetXpandDllPath(string path){
            while (Directory.GetDirectories(path).All(s => !string.Equals((new DirectoryInfo(s).Name + ""), "xpand.dll", StringComparison.OrdinalIgnoreCase))) {
                path = Path.GetFullPath(path + @"..\");
            }
            path += @"Xpand.dll";
            return path;
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB){
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[]{updater};
        }
    }
}