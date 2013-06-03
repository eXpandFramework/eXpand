using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;

namespace EFDemo.Module.Data {
    [DefaultClassOptions]
    public class Payment {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Decimal Rate { get; set; }
		public Decimal Hours { get; set; }
		
		[NotMapped]
		public Decimal Amount {
            get { return Rate * Hours; }
        }
    }
}
