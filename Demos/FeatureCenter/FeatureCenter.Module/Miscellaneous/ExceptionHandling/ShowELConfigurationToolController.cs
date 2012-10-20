using System;
using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace FeatureCenter.Module.Miscellaneous.ExceptionHandling {
    public class ShowELConfigurationToolController:ViewController<DetailView> {
        public ShowELConfigurationToolController() {
            TargetViewId = "ExceptionHandling_DetailView";
            var simpleAction = new SimpleAction(this, "Show EL Config Tool", PredefinedCategory.ObjectsCreation);
            simpleAction.Execute += SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            var fullPath = Path.GetFullPath("../../../../_third_party_assemblies/EntLibConfig.exe");
            string arguments = Path.GetFullPath("../../app.config");
            Process.Start(fullPath, arguments);
        }
    }
}