using System.ComponentModel;
using System.Drawing;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [InitialData(AllOwnMembers = true, BaseMembers = "oid|Oid", ThrowIfColumnNotFound = false)]
    [FullPermission]
    [DefaultProperty("Name")]
    public class Country : VideoRentalBaseObject {
        string _name;
        string _acronym;
        Image _flag;


        public Country(Session session)
            : base(session) {
        }
        [Indexed(Unique = true)]
        [RuleRequiredField]
        [RuleUniqueValue(DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Indexed(Unique = true)]
        public string Acronym {
            get { return _acronym; }
            set { SetPropertyValue("Acronym", ref _acronym, value); }
        }
        [ValueConverter(typeof(ImageValueConverter))]
        public Image Flag {
            get { return _flag; }
            set { SetPropertyValue("Flag", ref _flag, value); }
        }
        public override string ToString() {
            return Name ?? base.ToString();
        }

        [Association("Movies-Countries")]
        [InitialData]
        public XPCollection<Movie> Movies { get { return GetCollection<Movie>("Movies"); } }

        [Association("Companies-Country")]
        public XPCollection<Company> Companies {
            get { return GetCollection<Company>("Companies"); }
        }
    }
}