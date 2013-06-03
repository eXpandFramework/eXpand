using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace EFDemo.Module.Data {
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    public class Position_EF {
		public Position_EF() {
			Departments = new List<Department>();
			Contacts = new List<Contact>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[RuleRequiredField("RuleRequiredField for Position_EF.Title", DefaultContexts.Save)]
		public String Title { get; set; }
		public virtual IList<Department> Departments { get; set; }
		public virtual IList<Contact> Contacts { get; set; }
	}
}
