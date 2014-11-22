using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;

namespace FeatureCenter.Module.PivotChart {
    public class PivotChartController:ViewController<ObjectView>{
        public PivotChartController(){
            TargetObjectType = typeof (Analysis);
        }

        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<NewObjectViewController>().NewObjectAction.Active["14.2bug"]=false;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<NewObjectViewController>().NewObjectAction.Active["14.2bug"] = true;
        }
    }
}
