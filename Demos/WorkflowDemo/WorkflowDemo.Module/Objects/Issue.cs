using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
using DevExpress.ExpressApp;

namespace WorkflowDemo.Module.Objects {
    [DefaultClassOptions]
	[DefaultProperty("Subject")]
    [ImageName("BO_Note")]
    public class Issue : BaseObject {
		private string subject;
		private bool active;
        [Persistent]
        private User createdBy;

		public Issue(Session session) : base(session) { }
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
		public bool Active {
			get { return active; }
			set { SetPropertyValue("Active", ref active, value); }
		}
        public User CreatedBy {
            get { return createdBy; }
        }
    }
}
