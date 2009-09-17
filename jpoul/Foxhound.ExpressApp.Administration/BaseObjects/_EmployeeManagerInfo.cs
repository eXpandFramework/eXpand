using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    public class _EmployeeManagerInfo: BaseObjectInfo{
        [Association("EmployeeContract-ManagedUnits")]
        public XPCollection<_CompanyUnit> ManagedUnits
        {
            get
            {
                return GetCollection<_CompanyUnit>("ManagedUnits");
            }
        }
    }
}