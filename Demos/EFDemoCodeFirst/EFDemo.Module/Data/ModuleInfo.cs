using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp.Updating;

namespace EFDemo.Module.Data {
	[Browsable(false)]
	public class ModuleInfo : IModuleInfo {
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		public String Version { get; set; }
		public String AssemblyFileName { get; set; }
		public Boolean IsMain { get; set; }
	}
}
