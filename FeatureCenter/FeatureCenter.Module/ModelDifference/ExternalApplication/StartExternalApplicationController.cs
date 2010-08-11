using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace FeatureCenter.Module.ModelDifference.ExternalApplication
{
    public class StartExternalApplicationController:ViewController<DetailView>
    {
        public StartExternalApplicationController() {
            TargetViewId = "ExternalApplication_DetailView";
            var simpleAction = new SimpleAction(this, "Start External Application",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            Process.Start("ExternalApplication.Win.exe");
        }
    }
}
