using DevExpress.Xpo;
using DevExpress.Persistent.Base;
namespace eXpand.Persistent.TaxonomyImpl {
	public class Organization : Party {
		string profile = "";
		string email = "";
		string webSite = "";
		string description = "";
		public Organization(Session session) : base(session) { }
		public string Profile {
			get { return profile; }
			set { 
				profile = value;
				OnChanged("Profile");
			}
		}
		public string Email {
			get { return email; }
			set { 
				email = value;
				OnChanged("Email");
			}
		}
		public string WebSite {
			get { return webSite; }
			set {
				webSite = value;
				OnChanged("WebSite");
			}
		}
		[Size(4096), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string Description {
			get { return description; }
			set {
				description = value;
				OnChanged("Description"); 
			}
		}
	}
}
