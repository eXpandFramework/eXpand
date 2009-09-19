using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class Employee : TaxonomyBaseObject {
        
        public Employee(Session session) : base(session) { }
        [Association(Associations.JobDescriptionEmployees)]
        public XPCollection<JobDescription> JobDescriptions
        {
            get
            {
                return GetCollection<JobDescription>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        [Association(Associations.CompanyUnitEmployees)]
        public XPCollection<CompanyUnit> CompanyUnits
        {
            get
            {
                return GetCollection<CompanyUnit>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
    }
}
