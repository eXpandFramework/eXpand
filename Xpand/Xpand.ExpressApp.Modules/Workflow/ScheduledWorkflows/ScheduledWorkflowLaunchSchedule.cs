using System;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    [Appearance("WeekDays", "StartMode <> 'Weekly'",
        TargetItems = "RecurEveryWeeks;Monday;Tuesday;Wednesday;Thursday;Friday;Saturday;Sunday",
        Visibility = ViewItemVisibility.Hide)]
    public class ScheduledWorkflowLaunchSchedule : XpandCustomObject {
        public ScheduledWorkflowLaunchSchedule(Session session)
            : base(session) {
        }

        [Association("ScheduledWorkflow-LaunchScheduleItems")]
        public ScheduledWorkflow Workflow {
            get { return GetPropertyValue<ScheduledWorkflow>("Workflow"); }
            set { SetPropertyValue("Workflow", value); }
        }

        [ImmediatePostData]
        public StartMode StartMode {
            get { return GetPropertyValue<StartMode>("StartMode"); }
            set { SetPropertyValue("StartMode", value); }
        }

        public TimeSpan StartTime {
            get { return GetPropertyValue<TimeSpan>("StartTime"); }
            set { SetPropertyValue("StartTime", value); }
        }

        [ModelDefault("Caption", "Run if Missed")]
        public bool RuntASAPIfScheduledStartIsMissed {
            get { return GetPropertyValue<bool>("RuntASAPIfScheduledStartIsMissed"); }
            set { SetPropertyValue("RuntASAPIfScheduledStartIsMissed", value); }
        }

        [Appearance("RecurEveryDays", "StartMode <> 'Daily'", Visibility = ViewItemVisibility.Hide)]
        public int RecurEveryDays {
            get { return GetPropertyValue<int>("RecurEveryDays"); }
            set { SetPropertyValue("RecurEveryDays", value); }
        }

        [Appearance("RecurEveryMin", "StartMode <> 'Min'", Visibility = ViewItemVisibility.Hide)]
        public int RecurEveryMin {
            get { return GetPropertyValue<int>("RecurEveryMin"); }
            set { SetPropertyValue("RecurEveryMin", value); }
        }

        public int RecurEveryWeeks {
            get { return GetPropertyValue<int>("RecurEveryWeeks"); }
            set { SetPropertyValue("RecurEveryWeeks", value); }
        }

        public bool Monday {
            get { return GetPropertyValue<bool>("Monday"); }
            set { SetPropertyValue("Monday", value); }
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
            get { return GetPropertyValue<bool>("Saturday"); }
            set { SetPropertyValue("Saturday", value); }
        }

        public bool Sunday {
            get { return GetPropertyValue<bool>("Sunday"); }
            set { SetPropertyValue("Sunday", value); }
        }
    }
}