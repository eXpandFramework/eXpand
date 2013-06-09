using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;

namespace EFDemo.Module.Data {
    [DefaultClassOptions]
    [ModelDefault("Caption", "Task")]
    [Appearance("FontColorRed", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView", Criteria = "Status!='Completed'", FontColor = "Red")]
    public class DemoTask : Task {
        public DemoTask()
            : base() {
            Priority = Priority.Normal;
			Contacts = new List<Contact>();
        }

		[Browsable(false)]
		public Int32 Priority_Int { get; protected set; }
		public Nullable<Int32> EstimatedWork { get; set; }
		public Nullable<Int32> ActualWork { get; set; }
		public virtual IList<Contact> Contacts { get; set; }

		[NotMapped, Appearance("PriorityBackColorPink", AppearanceItemType = "ViewItem", Criteria = "Priority=2", BackColor = "0xfff0f0")]
		public Priority Priority {
			get { return (Priority)Priority_Int; }
			set { Priority_Int = (Int32)value; }
		}

		public override String ToString() {
            return Subject;
        }
		[Action(ToolTip = "Postpone the task to the next day", ImageName = "State_Task_Deferred")]
		public void Postpone() {
            if(DueDate == DateTime.MinValue) {
                DueDate = DateTime.Now;
            }
            DueDate = DueDate + TimeSpan.FromDays(1);
        }
    }
}
