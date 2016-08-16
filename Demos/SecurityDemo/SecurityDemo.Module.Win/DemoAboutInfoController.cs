using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.Utils.About;

namespace Demo.Module.Win {
	//For demo purposes only.
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
            DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.XAF, new DevExpress.Utils.About.ProductStringInfo("DXperience Universal", "eXpressApp Framework"));
        }
    }
}
