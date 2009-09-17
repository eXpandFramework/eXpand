using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base.General;
using Foxhound.ExpressApp.Scheduler.BaseObjects.Events;

namespace Foxhound.ExpressApp.Scheduler.Controllers{
    partial class SchedulerEventListViewController {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent(){
            this.components = new System.ComponentModel.Container();
            this.selectResourceTypeAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            //this.selectSchedulerEventViewAction = new XAFPoint.ExpressApp.Scheduler.Actions.SingleEnumerationChoiceAction(this.components);
            // 
            // selectResourceTypeAction
            // 
            this.selectResourceTypeAction.Caption = "Select Resource Type";
            this.selectResourceTypeAction.Category = "RecordEdit";
            this.selectResourceTypeAction.Id = "SelectResourceType";
            this.selectResourceTypeAction.Tag = null;
            this.selectResourceTypeAction.TargetObjectsCriteriaMode = DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode.TrueForAll;
            this.selectResourceTypeAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.selectResourceTypeAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.SelectResourceTypeActionExecuteHandler);
            // 
            // selectSchedulerEventViewAction
            // 
            //this.selectSchedulerEventViewAction.Caption = "Select Scheduler Event View";
            //this.selectSchedulerEventViewAction.Category = "RecordEdit";
            //this.selectSchedulerEventViewAction.EnumerationType = typeof(XAFPoint.ExpressApp.Scheduler.Controllers.SchedulerEventsView);
            //this.selectSchedulerEventViewAction.Id = "SelectSchedulerEventView";
            //this.selectSchedulerEventViewAction.Tag = null;
            //this.selectSchedulerEventViewAction.TargetObjectsCriteriaMode = DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode.TrueForAll;
            //this.selectSchedulerEventViewAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            //this.selectSchedulerEventViewAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.SelectSchedulerEventView_Execute);
            // 
            // SchedulerEventListViewController
            // 
            this.TargetObjectType = typeof(SchedulerEvent);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;

        }

        private SingleChoiceAction selectResourceTypeAction;
        //private Scheduler.Actions.SingleEnumerationChoiceAction selectSchedulerEventViewAction;
    }
}