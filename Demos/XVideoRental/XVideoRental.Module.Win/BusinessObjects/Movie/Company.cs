using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General.Model;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid", AllOwnMembers = true)]
    [CloneView(CloneViewType.ListView, "Company_Movies_ListView_Layout")]
    [RuleCombinationOfPropertiesIsUnique(DefaultContexts.Save, "Movie,Company")]
    public class MovieCompany : VideoRentalBaseObject {
        Company company;
        Movie movie;
        string description;

        public MovieCompany(Session session) : base(session) { }
        [Association("Movie-MovieCompany"),]
        public Movie Movie {
            get { return movie; }
            set { SetPropertyValue("Movie", ref movie, value); }
        }
        [Association("Company-MovieCompany"),]
        public Company Company {
            get { return company; }
            set { SetPropertyValue("Company", ref company, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }
    }

    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid")]
    public class CompanyType : VideoRentalBaseObject {
        string _name;

        public CompanyType(Session session) : base(session) { }

        [Indexed(Unique = true)]
        [InitialData]
        [RuleRequiredField]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
        public override string ToString() {
            return string.Format("{0}", Name);
        }
    }
    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid,Id|CompanyId", AllOwnMembers = true)]
    [ImageName("BO_Company")]
    [CloneView(CloneViewType.ListView, "Company_ListView_MasterDetail")]
    public class Company : VideoRentalBaseObject {
        string _name;
        CompanyType _type;
        string _webSite;
        Country _country;

        public Company(Session session)
            : base(session) {
        }

        [PersistentAlias("Id")]
        public long CompanyId {
            get { return (long)EvaluateAlias("CompanyId"); }

        }
        [RuleRequiredField]
        [Indexed(Unique = true)]
        [RuleUniqueValue(DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public CompanyType Type {
            get { return _type; }
            set { SetPropertyValue("Type", ref _type, value); }
        }

        public string WebSite {
            get { return _webSite; }
            set { SetPropertyValue("WebSite", ref _webSite, value); }
        }

        [Association("Companies-Country")]
        public Country Country {
            get { return _country; }
            set { SetPropertyValue("Country", ref _country, value); }
        }

        [Association("Company-MovieCompany"), Aggregated]
        public XPCollection<MovieCompany> Movies {
            get { return GetCollection<MovieCompany>("Movies"); }
        }
    }

}