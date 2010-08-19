using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Module.Miscellaneous.MultipleDataStore;

namespace FeatureCenter.Module.DetailViewControl.ActionButtonViewItem {
    public class ActionButtonViewItemController:ViewController<DetailView> {
        public ActionButtonViewItemController() {
            TargetObjectType = typeof (Customer);
            TargetViewId = "ActionButtonViewItem_DetailView";
            var simpleAction = new SimpleAction(this,"LastOrderLineDate",PredefinedCategory.Tools);
            simpleAction.Execute+=SimpleActionOnExecute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<DetailViewController>().SaveAction.ExecuteCompleted+=SaveActionOnExecuteCompleted;
            
        }

        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            throw new UserFriendlyException("Object is saved using Xaf Save Action");
        }



        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            DateTime dateTime = new XPQuery<MDSOrderLine>(ObjectSpace.Session).Max(line => line.OrderLineDate);
            ((Customer) View.CurrentObject).BirthDate=dateTime;
        }
    }
}