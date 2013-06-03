using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
    [DefaultProperty("Name")]
    public class Country : ICountry {
		public Country() {
			Addresses = new List<Address>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		public String PhoneCode { get; set; }
		public virtual IList<Address> Addresses { get; set; }
		
		public override String ToString() {
            return Name;
        }
    }
}
