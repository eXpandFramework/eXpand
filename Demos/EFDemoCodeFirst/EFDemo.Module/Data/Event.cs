using System;
using System.Xml;
using System.Data;
using System.Linq;
using System.Data.Objects;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.DC;

namespace EFDemo.Module.Data {
	[DefaultProperty("Subject")]
	[NavigationItem("Default")]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	public class Event : IEvent, IRecurrentEvent {
		private String resourceIds;
		private Boolean isUpdateResourcesDelayed;
		protected internal ObjectContext objectContext;

		private void UpdateResources() {
			Resources.Clear();
			if(!String.IsNullOrEmpty(resourceIds)) {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(resourceIds);
				foreach(XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
					EntityKey entityKey = new EntityKey(objectContext.DefaultContainerName + ".Resources", "Key", Int32.Parse(xmlNode.Attributes["Value"].Value));
					Object obj = null;
					if(objectContext.TryGetObjectByKey(entityKey, out obj)) {
						Resources.Add((Resource)obj);
					}
				}
			}
		}

		protected internal void BeforeSave() {
			if((objectContext != null) && isUpdateResourcesDelayed) {
				isUpdateResourcesDelayed = false;
				UpdateResources();
			}
		}

		public Event() {
			Resources = new List<Resource>();
			RecurrenceEvents = new List<Event>();
		}
		public void UpdateResourceIds() {
			resourceIds = "<ResourceIds>\r\n";
			foreach(Resource resource in Resources) {
				resourceIds += String.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Key.GetType().FullName, resource.Key);
			}
			resourceIds += "</ResourceIds>";
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[FieldSize(250)]
		public String Subject { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String Description { get; set; }
		public Nullable<DateTime> StartOn { get; set; }
		public Nullable<DateTime> EndOn { get; set; }
		public Boolean AllDay { get; set; }
		public String Location { get; set; }
		public Int32 Label { get; set; }
		public Int32 Status { get; set; }
		[Browsable(false)]
		public Int32 Type { get; set; }
		[NonCloneable, DisplayName("Recurrence"), FieldSize(FieldSizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String RecurrenceInfoXml { get; set; }
		public virtual IList<Resource> Resources { get; set; }
		[Browsable(false)]
		public Event RecurrencePattern { get; set; }
		[Browsable(false)]
		public virtual IList<Event> RecurrenceEvents { get; set; }

		[NotMapped, Browsable(false)]
		public String ResourceId {
			get {
				if(resourceIds == null) {
					UpdateResourceIds();
				}
				return resourceIds;
			}
			set {
				if(resourceIds != value) {
					resourceIds = value;
					if(objectContext != null) {
						UpdateResources();
					}
					else {
						isUpdateResourcesDelayed = true;
					}
				}
			}
		}
		[NotMapped, Browsable(false)]
		public Object AppointmentId {
			get { return ID; }
		}
		[NotMapped, Browsable(false), RuleFromBoolProperty("EventIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues = false, UsedProperties = "StartOn, EndOn")]
		public Boolean IsIntervalValid {
			get { return StartOn <= EndOn; }
		}

		IRecurrentEvent ISupportRecurrences.RecurrencePattern {
			get { return RecurrencePattern; }
			set { RecurrencePattern = (Event)value; }
		}
		DateTime IEvent.StartOn {
			get {
				if(StartOn.HasValue) {
					return StartOn.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { StartOn = value; }
		}
		DateTime IEvent.EndOn {
			get {
				if(EndOn.HasValue) {
					return EndOn.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { EndOn = value; }
		}
	}
}
