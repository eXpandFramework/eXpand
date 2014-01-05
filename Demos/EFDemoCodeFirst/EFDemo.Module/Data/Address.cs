using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
	[DefaultProperty("FullAddress")]
	public class Address : IAddress {
		private const string defaultFullAddressFormat = "{Country.Name}; {StateProvince}; {City}; {Street}; {ZipPostal}";
		public Address() {
			Parties1 = new List<Party>();
			Parties2 = new List<Party>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Street { get; set; }
		public String City { get; set; }
		public String StateProvince { get; set; }
		public String ZipPostal { get; set; }
		public Country Country { get; set; }
		[InverseProperty("Address1"), Browsable(false)]
		public virtual IList<Party> Parties1 { get; set; }
		[InverseProperty("Address2"), Browsable(false)]
		public virtual IList<Party> Parties2 { get; set; }

		[NotMapped]
		public String FullAddress {
			get { return ObjectFormatter.Format(AddressImpl.FullAddressFormat, this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty); }
		}

		ICountry IAddress.Country {
			get { return Country; }
			set { Country = value as Country; }
		}

		static Address() {
			AddressImpl.FullAddressFormat = defaultFullAddressFormat;
		}
		public static void SetFullAddressFormat(String format) {
			AddressImpl.FullAddressFormat = format;
		}
	}
}
