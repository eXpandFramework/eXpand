using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Web.Controls;
using Xpand.ExpressApp.PivotChart;

namespace FeatureCenter.Module.Web {
    public class MyClassC:ViewController {
        protected override void OnActivated() {
            base.OnActivated();
//            Frame.GetController<AnalysisDataBindController>()
        }
    }
    public class MyViewSiteControl : ViewSiteControl {
        public MyViewSiteControl() {
        }

        protected override void AddedControl(Control control, int index) {
            base.AddedControl(control, index);
        }
        protected override System.Web.UI.WebControls.Panel CreateControl() {
            return new MyPanel();
        }

        public object Tag { get; set; }
    }

    public class MyPanel : Panel {
        protected override void AddedControl(Control control, int index) {
            base.AddedControl(control, index);
        }

        protected override void RemovedControl(Control control) {
            base.RemovedControl(control);
        }

        public object Tag { get; set; }
    }

    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
        }
    }
}
