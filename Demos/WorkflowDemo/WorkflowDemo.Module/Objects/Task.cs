using System;
using System.Collections.Generic;
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
        [Persistent]
        private User createdBy;

		public Task(Session session) : base(session) { }
        public override void AfterConstruction() {
            base.AfterConstruction();
            if(SecuritySystem.CurrentUserId != null) {
                createdBy = (User)Session.GetObjectByKey(SecuritySystem.UserType, SecuritySystem.CurrentUserId);
            }
        }
        internal void SetCreatedBy(User user) {
            createdBy = user;
        }

		public string Subject {
			get { return subject; }
			set { SetPropertyValue("Subject", ref subject, value); }
		}
		public Issue Issue {
			get { return issue; }
			set { SetPropertyValue("Issue", ref issue, value); }
		}
        public User CreatedBy {
            get { return createdBy; }
        }
	}


}
