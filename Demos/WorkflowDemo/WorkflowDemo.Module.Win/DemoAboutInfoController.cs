using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.Utils.About;

namespace Demo.Module.Win {
    public class DemoAboutInfoController : AboutInfoController {
        private SimpleAction demoAboutInfoAction;

        public DemoAboutInfoController()
            : base() {
            demoAboutInfoAction = new SimpleAction(this, "Demo About Info", DevExpress.Persistent.Base.PredefinedCategory.About);
            demoAboutInfoAction.Caption = "About...";
        }
        protected override void OnActivated() {
            base.OnActivated();
            demoAboutInfoAction.ImageName = AboutInfoAction.ImageName;
            AboutInfoAction.Active["DemoDisabled"] = false;
            demoAboutInfoAction.Execute += new SimpleActionExecuteEventHandler(demoAboutInfoAction_Execute);
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            demoAboutInfoAction.Execute -= new SimpleActionExecuteEventHandler(demoAboutInfoAction_Execute);
        }

        private void demoAboutInfoAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            AboutForm.Show(typeof(string), ProductKind.XAF, DevExpress.Utils.About.ProductInfoStage.Registered);
        }
    }
}
