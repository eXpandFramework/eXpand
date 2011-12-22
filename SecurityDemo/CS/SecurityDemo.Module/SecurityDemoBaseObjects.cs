using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using System.Security;
using DevExpress.Data.Filtering;
using System.Text;
using System;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;

namespace SecurityDemo.Module {
	[NonPersistent]
	public abstract class SecurityDemoBaseObject : BaseObject {
        private string name;
		public SecurityDemoBaseObject(Session session)
			: base(session) {
		}
        public string Name {
            get {
                return name;
            }
            set {
                SetPropertyValue("Name", ref name, value);
            }
        }
    }
}
