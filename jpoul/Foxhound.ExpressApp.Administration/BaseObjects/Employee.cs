using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class Employee : TaxonomyBaseObject {
        
        public Employee(Session session) : base(session) { }

        private CompanyUnit companyUnit;
        [Association(Associations.CompanyUnitEmployees)]
        public CompanyUnit CompanyUnit
        {
            get
            {
                return companyUnit;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref companyUnit, value);
            }
        }
    }
}
