using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Security;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp;

namespace SecurityDemo.Module {
    [Hint("", ViewType.DetailView, "Description")]
    public class SecurityDemoUser : SecurityUser {
   		private string description;
        public SecurityDemoUser(Session session) : base(session) { }
		[Browsable(false), Size(1024)]
		public string Description {
			get { return description; }
			set { SetPropertyValue("Description", ref description, value); }
		}
    }
}
