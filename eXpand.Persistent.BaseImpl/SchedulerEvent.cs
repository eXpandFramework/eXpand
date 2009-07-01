using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl
{
    [DefaultProperty("Label")]
    [NavigationItem(false)]
    public class SchedulerEvent : BaseObject, IEvent, IHasResources {
        public ResourceHelper ResourceHelper { get; private set;}
        private PersistentDateRangeInfo relatedInfo;
        
        public SchedulerEvent(Session session) : base(session){
            ResourceHelper = new ResourceHelper(this);
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            appointmentImpl.AfterConstruction();
        }
        [Association("PersistentDateRangeInfo-RelatedEvents")]
        public PersistentDateRangeInfo RelatedInfo {
            get {
                return relatedInfo;
            }
            set {
                SetPropertyValue("RelatedInfo", ref relatedInfo, value);
            }
        }

        #region Validations
        [NonPersistent]
        [Browsable(false)]
        [RuleFromBoolProperty("EventBaseIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", UsedProperties = "StartOn, EndOn")]
        public bool IsIntervalValid{
            get { return StartOn < EndOn; }
        }
        #endregion
        #region Recurrence
        protected string recurrenceInfoXml;
        [Persistent("RecurrencePattern")] protected Event recurrencePattern;

        [DevExpress.Xpo.DisplayName("TemplateRecurrenceType"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public string RecurrenceInfoXml{
            get { return recurrenceInfoXml; }
            set{
                recurrenceInfoXml = value;
                OnChanged("RecurrenceInfoXml");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public IEvent RecurrencePattern{
            get { return recurrencePattern; }
            set{
                recurrencePattern = (Event) value;
                OnChanged("RecurrencePattern");
            }
        }
        #endregion
        #region IEvent Implementation
        protected EventImpl appointmentImpl = new EventImpl();

        [NonPersistent, Browsable(false)]
        public string AppointmentId{
            get { return Oid.ToString(); }
        }

        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public string Description{
            get { return appointmentImpl.Description; }
            set{
                appointmentImpl.Description = value;
                OnChanged("Description");
            }
        }

        [Indexed]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public DateTime StartOn{
            get { return appointmentImpl.StartOn; }
            set{
                appointmentImpl.StartOn = value;
                OnChanged("StartOn");
            }
        }

        [Indexed]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public DateTime EndOn{
            get { return appointmentImpl.EndOn; }
            set{
                appointmentImpl.EndOn = value;
                OnChanged("EndOn");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool AllDay{
            get { return appointmentImpl.AllDay; }
            set{
                appointmentImpl.AllDay = value;
                OnChanged("AllDay");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public string Location{
            get { return appointmentImpl.Location; }
            set{
                appointmentImpl.Location = value;
                OnChanged("Location");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public int Label{
            get { return appointmentImpl.Label; }
            set{
                appointmentImpl.Label = value;
                OnChanged("Label");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public int Status{
            get { return appointmentImpl.Status; }
            set{
                appointmentImpl.Status = value;
                OnChanged("Status");
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public int Type{
            get { return appointmentImpl.Type; }
            set{
                appointmentImpl.Type = value;
                OnChanged("Type");
            }
        }

        [Size(250)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public string Subject{
            get { return appointmentImpl.Subject; }
            set{
                appointmentImpl.Subject = value;
                OnChanged("Subject", null, value);
            }
        }

        [Persistent, Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        public string ResourceId{
            get { return ResourceHelper.ResourceId; }
            set{
                ResourceHelper.ResourceId = value;
                OnChanged("ResourceId");
            }
        }
        #endregion
    }
}