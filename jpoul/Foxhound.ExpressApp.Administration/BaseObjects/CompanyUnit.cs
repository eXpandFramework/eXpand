using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class CompanyUnit : Organization{
        public CompanyUnit(Session session)
            : base(session) {}
    }
}