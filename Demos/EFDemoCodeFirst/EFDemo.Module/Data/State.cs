using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace EFDemo.Module.Data {
	[DefaultProperty("LongName")]
	public class State {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String ShortName { get; set; }
		public String LongName { get; set; }
	}
}
