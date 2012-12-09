using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace XVideoRental.Module.Win.BusinessObjects.Rent {
    [Persistent("Rent")]
    public abstract class RentEvent : VideoRentalBaseObject, IEvent {
        [Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        private String _resourceids;

        private readonly EventImpl appointmentImpl = new EventImpl();

        void UpdateResources() {
            Resources.SuspendChangedEvents();
            try {
                while (Resources.Count > 0) {
                    Resources.Remove(Resources[0]);
                }
                if (!String.IsNullOrEmpty(_resourceids)) {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(_resourceids);
                    if (xmlDocument.DocumentElement != null)
                        foreach (Customer resource in GetResources(xmlDocument)) {
                            Resources.Add(resource);
                        }
                }
            } finally {
                Resources.ResumeChangedEvents();
            }
        }

        IEnumerable<Customer> GetResources(XmlDocument xmlDocument) {
            if (xmlDocument.DocumentElement != null)
                return from XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes
                       let xmlAttributeCollection = xmlNode.Attributes
                       where xmlAttributeCollection != null
                       where xmlAttributeCollection != null
                       select Session.GetObjectByKey<Customer>(new Guid(xmlAttributeCollection["Value"].Value))
                           into resource
                           where resource != null select resource;
            return Enumerable.Empty<Customer>();
        }

        private void Resources_CollectionChanged(object sender, XPCollectionChangedEventArgs e) {
            if ((e.CollectionChangedType == XPCollectionChangedType.AfterAdd) || (e.CollectionChangedType == XPCollectionChangedType.AfterRemove)) {
                UpdateResourceIds();
                OnChanged("ResourceId");
            }
        }
        private void session_ObjectSaving(object sender, ObjectManipulationEventArgs e) {
        }
        protected override XPCollection CreateCollection(XPMemberInfo property) {
            XPCollection result = base.CreateCollection(property);
            if (property.Name == "Resources") {
                result.CollectionChanged += Resources_CollectionChanged;
            }
            return result;
        }

        protected RentEvent(Session session)
            : base(session) {
            session.ObjectSaving += session_ObjectSaving;
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            appointmentImpl.AfterConstruction();
        }
        public void UpdateResourceIds() {
            _resourceids = "<ResourceIds>\r\n";
            foreach (Customer resource in Resources) {
                _resourceids += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
            }
            _resourceids += "</ResourceIds>";
        }
        [NonPersistent, Browsable(false)]
        public object AppointmentId {
            get { return Id; }
        }
        [Size(250)]
        [NonPersistent]
        public virtual string Subject {
            get { return appointmentImpl.Subject; }
            set {
                appointmentImpl.Subject = value;
                OnChanged("Subject");
            }
        }
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [NonPersistent]
        public virtual string Description {
            get { return appointmentImpl.Description; }
            set {
                appointmentImpl.Description = value;
                OnChanged("Description");
            }
        }
        [Indexed]
        [NonPersistent]
        public virtual DateTime StartOn {
            get { return appointmentImpl.StartOn; }
            set {
                appointmentImpl.StartOn = value;
                OnChanged("StartOn");
            }
        }
        [Indexed]
        [NonPersistent]
        public virtual DateTime EndOn {
            get { return appointmentImpl.EndOn; }
            set {
                appointmentImpl.EndOn = value;
                OnChanged("EndOn");
            }
        }
        public bool AllDay {
            get { return appointmentImpl.AllDay; }
            set {
                appointmentImpl.AllDay = value;
                OnChanged("AllDay");
            }
        }
        public string Location {
            get { return appointmentImpl.Location; }
            set {
                appointmentImpl.Location = value;
                OnChanged("Location");
            }
        }
        [NonPersistent]
        public virtual int Label {
            get { return appointmentImpl.Label; }
            set {
                appointmentImpl.Label = value;
                OnChanged("Label");
            }
        }
        [NonPersistent]
        public virtual int Status {
            get { return appointmentImpl.Status; }
            set {
                appointmentImpl.Status = value;
                OnChanged("Status");
            }
        }
        [NonPersistent]
        public int Type {
            get { return appointmentImpl.Type; }
            set {
                appointmentImpl.Type = value;
                OnChanged("Type");
            }
        }

        public XPCollection Resources {
            get {
                var receipt = (Receipt)GetMemberValue("Receipt");
                return new XPCollection(Session, typeof(Customer), false) { receipt.Customer };
            }
        }

        [NonPersistent, Browsable(false)]
        public String ResourceId {
            get {
                if (_resourceids == null) {
                    UpdateResourceIds();
                }
                return _resourceids;
            }
            set {
                if (_resourceids != value) {
                    _resourceids = value;
                    UpdateResources();
                }
            }
        }

        [NonPersistent]
        [Browsable(false)]
        [RuleFromBoolProperty("MyEventIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", UsedProperties = "StartOn, EndOn")]
        public bool IsIntervalValid { get { return StartOn < EndOn; } }

    }
}
