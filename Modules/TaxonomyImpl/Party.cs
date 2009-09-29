
using System;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl {
	[MapInheritance(MapInheritanceType.OwnTable)]
	[DefaultProperty("Caption")]
    public abstract class Party : BaseObject
    {
		private Address address1;
		private Address address2;
		protected Party(Session session) : base(session) { }
		public override string ToString() {
			return Caption;
		}
		[Size(SizeAttribute.Unlimited), Delayed, ValueConverter(typeof(ImageValueConverter))]
		public Image Photo {
			get { return GetDelayedPropertyValue<Image>("Photo"); }
			set { SetDelayedPropertyValue("Photo", value); }
		}
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public Address Address1 {
			get { return address1; }
			set { 
				address1 = value;
				OnChanged("Address1");
			}
		}
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public Address Address2 {
			get { return address2; }
			set { 
				address2 = value;
				OnChanged("Address2");
			}
		}
        
        [Aggregated, Association("Party-PhoneNumbers")]
		public XPCollection<PhoneNumber> PhoneNumbers {
			get { return GetCollection<PhoneNumber>("PhoneNumbers"); }
		}
	}
}
