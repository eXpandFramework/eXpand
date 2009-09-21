#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                 }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2009 Developer Express Inc.

using System;
using System.ComponentModel;
using System.Xml;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo.Metadata;
using System.Text;
using DevExpress.Data.Filtering;
using System.Xml.Serialization;
using System.IO;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.TaxonomyImpl {
	[DefaultProperty("Subject")]
	public class Event : BaseObject, IEvent, ISupportRecurrences {
#if MediumTrust
		[Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String resourceIds;
#else
		[Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		private String resourceIds;
#endif
		private EventImpl appointmentImpl = new EventImpl();
		[Persistent("RecurrencePattern")]
		private Event recurrencePattern;
		private string recurrenceInfoXml;
		private void UpdateResources() {
			Resources.SuspendChangedEvents();
			try {
				while(Resources.Count > 0) {
					Resources.Remove(Resources[0]);
				}
				if(!String.IsNullOrEmpty(resourceIds)) {
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(resourceIds);
					foreach(XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
						Resource resource = Session.GetObjectByKey<Resource>(new Guid(xmlNode.Attributes["Value"].Value));
						if(resource != null) {
							Resources.Add(resource);
						}
					}
				}
			}
			finally {
				Resources.ResumeChangedEvents();
			}
		}
		private void Resources_ListChanged(object sender, ListChangedEventArgs e) {
			if((e.ListChangedType == ListChangedType.ItemAdded) ||
				(e.ListChangedType == ListChangedType.ItemDeleted) ||
				(e.ListChangedType == ListChangedType.Reset)) {
				UpdateResourceIds();
				OnChanged("ResourceId");
			}
		}
		private void session_ObjectSaving(object sender, ObjectManipulationEventArgs e) {
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			Resources.ListChanged -= new ListChangedEventHandler(Resources_ListChanged);
			if(Resources.IsLoaded) {
				Resources.Reload();
			}
			Resources.ListChanged += new ListChangedEventHandler(Resources_ListChanged);
		}
		public Event(Session session)
			: base(session) {
			session.ObjectSaving += new ObjectManipulationEventHandler(session_ObjectSaving);
		}
		public override void AfterConstruction() {
			base.AfterConstruction();
			appointmentImpl.AfterConstruction();
		}
		public void UpdateResourceIds() {
			resourceIds = "<ResourceIds>\r\n";
			foreach(Resource resource in Resources) {
				resourceIds += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
			}
			resourceIds += "</ResourceIds>";
		}
		[NonPersistent, Browsable(false)]
		public string AppointmentId {
			get { return Oid.ToString(); }
		}
		[Size(250)]
		public string Subject {
			get { return appointmentImpl.Subject; }
			set {
				appointmentImpl.Subject = value;
				OnChanged("Subject", null, value);
			}
		}
		[Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string Description {
			get { return appointmentImpl.Description; }
			set {
				appointmentImpl.Description = value;
				OnChanged("Description");
			}
		}
		[Indexed]
		public DateTime StartOn {
			get { return appointmentImpl.StartOn; }
			set {
				appointmentImpl.StartOn = value;
				OnChanged("StartOn");
			}
		}
		[Indexed]
		public DateTime EndOn {
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
		public int Label {
			get { return appointmentImpl.Label; }
			set {
				appointmentImpl.Label = value;
				OnChanged("Label");
			}
		}
		public int Status {
			get { return appointmentImpl.Status; }
			set {
				appointmentImpl.Status = value;
				OnChanged("Status");
			}
		}
		public int Type {
			get { return appointmentImpl.Type; }
			set {
				appointmentImpl.Type = value;
				OnChanged("Type");
			}
		}
		[Association("Event-Resource", typeof(Resource))]
		public XPCollection Resources {
			get { return GetCollection("Resources"); }
		}
		[NonPersistent(), Browsable(false)]
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
					UpdateResources();
				}
			}
		}
		[DevExpress.Xpo.DisplayName("Recurrence"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string RecurrenceInfoXml {
			get {
				return recurrenceInfoXml;
			}
			set {
				recurrenceInfoXml = value;
				OnChanged("RecurrenceInfoXml");
			}
		}
		public IEvent RecurrencePattern {
			get {
				return recurrencePattern;
			}
			set {
				recurrencePattern = (Event)value;
				OnChanged("RecurrencePattern");
			}
		}
		[NonPersistent]
		[Browsable(false)]
		[RuleFromBoolProperty("EventIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", UsedProperties = "StartOn, EndOn")]
		public bool IsIntervalValid { get { return StartOn < EndOn; } }
	}
}
