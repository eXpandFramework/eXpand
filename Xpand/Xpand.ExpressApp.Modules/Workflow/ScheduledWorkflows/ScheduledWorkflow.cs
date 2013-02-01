using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    public enum StartMode {
        OneTime, Daily, Weekly
    }

    [DefaultClassOptions]
    [NavigationItem("Workflow")]

    public class ScheduledWorkflow : XpandCustomObject, IXpandWorkflowDefinition {
        public ScheduledWorkflow(Session session) : base(session) { }

        public bool IsActive {
            get { return GetPropertyValue<bool>("IsActive"); }
            set { SetPropertyValue("IsActive", value); }
        }

        #region Scheduled properties

        [Association("ScheduledWorkflow-LaunchScheduleItems"), Aggregated]
        public XPCollection<ScheduledWorkflowLaunchSchedule> LaunchScheduleItems {
            get { return GetCollection<ScheduledWorkflowLaunchSchedule>("LaunchScheduleItems"); }
        }

        [Association]
        public XPCollection<ScheduledWorkflowLaunchHistory> LaunchHistoryItems {
            get { return GetCollection<ScheduledWorkflowLaunchHistory>("LaunchHistoryItems"); }
        }

        #endregion

        #region IWorkflowDefinition Members
        public string GetActivityTypeName() {
            return GetUniqueId();
        }
        public bool CanCompileForDesigner { get; set; }
        public IList<IStartWorkflowCondition> GetConditions() {
            return new IStartWorkflowCondition[0];
        }

        public string GetUniqueId() {
            if (Session.IsNewObject(this))
                throw new InvalidOperationException();
            return "ScheduledWorkflow" + Oid.ToString().ToUpper().Replace("-", "_");
        }

        [Browsable(false)]
        public bool CanCompile {
            get { return false; }
        }

        [Browsable(false)]
        public bool CanOpenHost {
            get { return IsActive && !string.IsNullOrEmpty(Name); }
        }

        public string Name {
            get { return GetPropertyValue<string>("Name"); }
            set { SetPropertyValue("Name", value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("PropertyEditorType", "DevExpress.ExpressApp.Workflow.Win.WorkflowPropertyEditor")]
        public string Xaml {
            get { return GetPropertyValue<string>("Xaml"); }
            set { SetPropertyValue("Xaml", value); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Xaml = DCWorkflowDefinitionLogic.InitialXaml;
        }
        #endregion

    }
}