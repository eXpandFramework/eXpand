using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Organization=eXpand.Persistent.TaxonomyImpl.Organization;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class CompanyUnit : Organization {
        public CompanyUnit(Session session) : base(session){
        }
        [Association(Associations.CompanyUnitEmployees)]
        public XPCollection<Employee> Employees
        {
            get
            {
                return GetCollection<Employee>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        private Company company;
        [Association(Associations.CompanyCompanyUnits)]
        public Company Company
        {
            get
            {
                return company;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref company, value);
            }
        }
    }
}