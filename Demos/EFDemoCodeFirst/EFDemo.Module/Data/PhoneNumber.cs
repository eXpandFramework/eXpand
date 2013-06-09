using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
    [DefaultProperty("Number")]
    public class PhoneNumber : IPhoneNumber {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Number { get; set; }
		public String PhoneType { get; set; }
		public Party Party { get; set; }
		
		public override String ToString() {
            return Number;
        }
    }
}
