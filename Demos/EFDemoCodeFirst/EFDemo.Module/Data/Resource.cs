using System;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
	[DefaultProperty("Caption")]
	public class Resource : IResource {
		public Resource() {
			Events = new List<Event>();
		}
		[Key, Browsable(false)]
		public Int32 Key { get; protected set; }
		public String Caption { get; set; }
		[Browsable(false)]
		public Int32 Color_Int { get; protected set; }
		public virtual IList<Event> Events { get; set; }
		
		[NotMapped, Browsable(false)]
		public Object Id {
			get { return Key; }
		}
		[NotMapped, Browsable(false)]
		public Int32 OleColor {
			get {
				return ColorTranslator.ToOle(Color.FromArgb(Color_Int));
			}
		}
		[NotMapped]
		public Color Color {
			get { return Color.FromArgb(Color_Int); }
			set { Color_Int = value.ToArgb(); }
		}
	}
}
