using System;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace EFDemo.Module.Data {
	[DefaultClassOptions]
	public class Contact : Person {
		public Contact() {
			Resumes = new List<Resume>();
			Contacts = new List<Contact>();
			Tasks = new List<DemoTask>();
		}

		public String WebPageAddress { get; set; }
		public String NickName { get; set; }
		public String SpouseName { get; set; }
		[Browsable(false)]
		public Int32 TitleOfCourtesy_Int { get; protected set; }
		public Nullable<DateTime> Anniversary { get; set; }
		[FieldSize(4096)]
		public String Notes { get; set; }
		[ImmediatePostData]
		public virtual Department Department { get; set; }
		public virtual Position_EF Position { get; set; }
		public virtual IList<Resume> Resumes { get; set; }
		[DataSourceProperty("Department.Contacts", DataSourcePropertyIsNullMode.SelectAll), DataSourceCriteria("Position_EF.Title = 'Manager'")]
		public virtual Contact Manager { get; set; }
		public virtual IList<Contact> Contacts { get; set; }
		public virtual IList<DemoTask> Tasks { get; set; }

		[NotMapped]
		public TitleOfCourtesy TitleOfCourtesy {
			get { return (TitleOfCourtesy)TitleOfCourtesy_Int; }
			set { TitleOfCourtesy_Int = (Int32)value; }
		}
		[NotMapped]
		public Image Photo {
			get { return GetPhoto(); }
			set { SetPhoto(value); }
		}
	}
}
