using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [InitialData(BaseMembers = "oid|Oid")]
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    [ImageName("BO_Languages")]
    public class Language : VideoRentalBaseObject {
        string name;
        public Language(Session session) : base(session) { }
        public Language(Session session, string name)
            : this(session) {
            Name = name;
        }
        [Indexed(Unique = true)]
        [InitialData]
        [RuleRequiredField]
        [RuleUniqueValue(DefaultContexts.Save)]
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        [Association("Movies-Language")]
        public XPCollection<Movie> Movies { get { return GetCollection<Movie>("Movies"); } }
    }
}