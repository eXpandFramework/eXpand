using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class Employee : BaseObject {
        public Employee(Session session) : base(session) {}
    }
}
