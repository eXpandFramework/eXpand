using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.Persistent.Base;

namespace EFDemo.Module.Data {
    [DefaultClassOptions]
    [ImageName("BO_Resume")]
    public class Resume {
		public Resume() {
			Portfolio = new List<PortfolioFileData>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public virtual IList<PortfolioFileData> Portfolio { get; set; }
		public virtual Contact Contact { get; set; }
	}
}
