using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace WorkflowDemo.Module.Objects {
    [DefaultClassOptions]
	[DefaultProperty("Subject")]
    [ImageName("BO_Note")]
    public class Issue : BaseObject {
		private string subject;
		private bool active;
		public Issue(Session session) : base(session) { }
		public string Subject {
			get { return subject; }
			set { SetPropertyValue("Subject", ref subject, value); }
		}
		public bool Active {
			get { return active; }
			set { SetPropertyValue("Active", ref active, value); }
		}
    }
}
