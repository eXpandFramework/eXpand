using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DevExpress.Persistent.Base;

namespace EFDemo.Module.Data {
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    public class Department {
		public Department() {
			Positions = new List<Position_EF>();
			Contacts = new List<Contact>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Title { get; set; }
		public String Office { get; set; }
		public virtual IList<Position_EF> Positions { get; set; }
		public virtual IList<Contact> Contacts { get; set; }
	}
}
