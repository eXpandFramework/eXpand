using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
	[DefaultProperty("Subject")]
	public class Task : ITask {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Nullable<DateTime> DateCompleted { get; protected set; }
		public String Subject { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String Description { get; set; }
		public Nullable<DateTime> DueDate { get; set; }
		public Nullable<DateTime> StartDate { get; set; }
		[Browsable(false)]
		public Int32 Status_Int { get; set; }
		public Int32 PercentCompleted { get; set; }
		public Party AssignedTo { get; set; }

		[NotMapped]
		public TaskStatus Status {
			get { return (TaskStatus)Status_Int; }
			set {
				Status_Int = (Int32)value;
				if(value == TaskStatus.Completed) {
					DateCompleted = DateTime.Now;
				}
				else {
					DateCompleted = null;
				}
			}
		}

		// ITask
		DateTime ITask.DateCompleted {
			get {
				if(DateCompleted.HasValue) {
					return DateCompleted.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
		}
		DateTime ITask.DueDate {
			get {
				if(DueDate.HasValue) {
					return DueDate.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { DueDate = value; }
		}
		DateTime ITask.StartDate {
			get {
				if(StartDate.HasValue) {
					return StartDate.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { StartDate = value; }
		}

		[Action(ImageName = "State_Task_Completed")]
		public void MarkCompleted() {
			Status = TaskStatus.Completed;
		}
	}
}
