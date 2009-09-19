using System.Reflection;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using DevExpress.Xpo;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class JobDescription : TaxonomyBaseObject{
        [Association(Associations.JobDescriptionCompanyUnits)]
        public XPCollection<CompanyUnit> CompanyUnits
        {
            get
            {
                return GetCollection<CompanyUnit>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        [Association(Associations.JobDescriptionEmployees)]
        public XPCollection<Employee> Employees
        {
            get
            {
                return GetCollection<Employee>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
    }
}