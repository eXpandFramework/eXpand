using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.ExpressApp;

namespace WorkflowDemo.Module.Objects {
	[DefaultClassOptions]
    [ImageName("BO_Task")]
    public class Task : BaseObject {
		private string subject;
		private Issue issue;
		public Task(Session session) : base(session) { }
		public string Subject {
			get { return subject; }
			set { SetPropertyValue("Subject", ref subject, value); }
		}
		public Issue Issue {
			get { return issue; }
			set { SetPropertyValue("Issue", ref issue, value); }
		}
	}
}
