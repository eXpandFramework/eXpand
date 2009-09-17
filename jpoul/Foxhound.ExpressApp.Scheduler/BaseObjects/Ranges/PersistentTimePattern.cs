using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Foxhound.Persistent.BaseImpl;
using Foxhound.Xpo.Converters.ValueConverters;

namespace Foxhound.ExpressApp.Scheduler.BaseObjects.Ranges{
    [NavigationItem(false)]
    public class PersistentTimePattern : BaseObject{
        private bool isDefaultForTotalHoursAndComposition;
        [ValueConverter(typeof (SerializableObjectConverter))] [Persistent("TimePattern"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))] protected TimePattern timePattern;

        public PersistentTimePattern(Session session) : base(session) {}

        [Browsable(false)]
        public TimeRange Range{
            get { return timePattern[0]; }
        }

        [NonPersistent]
        public TimeSpan RangeStartTime{
            get { return Range.StartTime; }
            set { Range.StartTime = value; }
        }

        [Persistent]
        [Browsable(false)]
        public TimeSpan RangeEndTime{
            get { return Range.EndTime; }
            set { Range.EndTime = value; }
        }

        [Persistent]
        [Index(5)]
        public TimeSpan Duration{
            get { return timePattern.Duration; }
        }

        [Persistent]
        [Browsable(false)]
        public double TotalHours{
            get { return timePattern.TotalHours; }
        }

        [Persistent]
        [Index(0)]
        public string Caption{
            get { return timePattern.Caption; }
        }

        [Persistent]
        [Browsable(false)]
        public TimeSpan StartTime{
            get { return timePattern.StartTime; }
        }

        [Persistent]
        [Browsable(false)]
        public TimeSpan EndTime{
            get { return timePattern.EndTime; }
        }

        [RuleFromBoolProperty("PersistentTimePatternIsEmpty", DefaultContexts.Save, InvertResult = true)]
        [Browsable(false)]
        public bool IsEmpty{
            get { return timePattern.IsEmpty; }
        }

        [Persistent]
        [Browsable(false)]
        public bool IsComposite{
            get { return timePattern.IsComposite; }
        }

        [Index(6)]
        public bool IsDefaultForTotalHoursAndComposition{
            get { return isDefaultForTotalHoursAndComposition; }
            set { SetPropertyValue("IsDefaultForTotalHoursAndComposition", ref isDefaultForTotalHoursAndComposition, value); }
        }

        [Browsable(false)]
        public TimePattern TimePattern{
            get { return timePattern; }
        }

        public override void AfterConstruction(){
            timePattern = new TimePattern(new[]{new TimeRange()});
        }

        public static TDateRange GetDefaultPatternForDuration<TDateRange>(Session session, double totalHours) where TDateRange : PersistentTimePattern{
            return (from pat in new XPQuery<TDateRange>(session)
                    where pat.TotalHours == totalHours && pat.IsDefaultForTotalHoursAndComposition
                    select pat).SingleOrDefault();
        }
    }
}