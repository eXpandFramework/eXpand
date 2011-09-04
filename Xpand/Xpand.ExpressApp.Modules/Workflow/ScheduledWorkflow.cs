using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Workflow {
    public enum StartMode {
        OneTime,
        Daily,
        Weekly
    }

    [DefaultClassOptions]
    [NavigationItem("Workflow")]
    [Appearance("WeekDays", "StartMode <> 'Weekly'",
        TargetItems = "RecurEveryWeeks;Moday;Tuesday;Wednesday;Thursday;Friday;Saturday;Sunday",
        Visibility = ViewItemVisibility.Hide)]
    public class ScheduledWorkflow : XpandCustomObject, IWorkflowDefinition {
        public ScheduledWorkflow(Session session)
            : base(session) {
        }

        public bool IsActive {
            get { return GetPropertyValue<bool>("IsActive"); }
            set { SetPropertyValue("IsActive", value); }
        }



        #region Scheduled properties
        [ImmediatePostData]
        public StartMode StartMode {
            get { return GetPropertyValue<StartMode>("StartMode"); }
            set { SetPropertyValue("StartMode", value); }
        }

        public TimeSpan StartTime {
            get { return GetPropertyValue<TimeSpan>("StartTime"); }
            set { SetPropertyValue("StartTime", value); }
        }
        public bool RuntASAPIfScheduledStartIsMissed {
            get { return GetPropertyValue<bool>("RuntASAPIfScheduledStartIsMissed"); }
            set { SetPropertyValue("RuntASAPIfScheduledStartIsMissed", value); }
        }
        [Appearance("RecurEveryDays", "StartMode <> 'Daily'", Visibility = ViewItemVisibility.Hide)]
        public int RecurEveryDays {
            get { return GetPropertyValue<int>("RecurEveryDays"); }
            set { SetPropertyValue("RecurEveryDays", value); }
        }

        public int RecurEveryWeeks {
            get { return GetPropertyValue<int>("RecurEveryWeeks"); }
            set { SetPropertyValue("RecurEveryWeeks", value); }
        }

        public bool Moday {
            get { return GetPropertyValue<bool>("Moday"); }
            set { SetPropertyValue("Moday", value); }
        }

        [Association]
        public XPCollection<ScheduledWorkflowLaunchHistory> LaunchHistoryItems {
            get { return GetCollection<ScheduledWorkflowLaunchHistory>("LaunchHistoryItems"); }
        }

        public bool Tuesday {
            get { return GetPropertyValue<bool>("Tuesday"); }
            set { SetPropertyValue("Tuesday", value); }
        }

        public bool Wednesday {
            get { return GetPropertyValue<bool>("Wednesday"); }
            set { SetPropertyValue("Wednesday", value); }
        }

        public bool Thursday {
            get { return GetPropertyValue<bool>("Thursday"); }
            set { SetPropertyValue("Thursday", value); }
        }

        public bool Friday {
            get { return GetPropertyValue<bool>("Friday"); }
            set { SetPropertyValue("Friday", value); }
        }

        public bool Saturday {
            get { return GetPropertyValue<bool>("Moday"); }
            set { SetPropertyValue("Saturday", value); }
        }

        public bool Sunday {
            get { return GetPropertyValue<bool>("Sunday"); }
            set { SetPropertyValue("Sunday", value); }
        }
        #endregion

        #region IWorkflowDefinition Members
        public string GetActivityTypeName() {
            return GetUniqueId();
        }

        public IList<IStartWorkflowCondition> GetConditions() {
            return new IStartWorkflowCondition[0];
        }

        public string GetUniqueId() {
            if (Session.IsNewObject(this)) {
                throw new InvalidOperationException();
            }
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
        [Custom("PropertyEditorType", "DevExpress.ExpressApp.Workflow.Win.WorkflowPropertyEditor")]
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