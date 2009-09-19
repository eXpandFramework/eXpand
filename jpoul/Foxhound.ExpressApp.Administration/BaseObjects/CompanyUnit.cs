using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using Organization=eXpand.Persistent.TaxonomyImpl.Organization;
using System.Linq;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class CompanyUnit : Organization {
        public CompanyUnit(Session session) : base(session){
        }
        [Association(Associations.JobDescriptionCompanyUnits)]
        public XPCollection<JobDescription> JobDescriptions
        {
            get
            {
                return GetCollection<JobDescription>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        [Association(Associations.CompanyUnitEmployees)]
        public XPCollection<Employee> Employees
        {
            get
            {
                return GetCollection<Employee>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
//        private Term term;
//        [NonPersistent]
//        public Term Term
//        {
//            get
//            {
//                return term;
//            }
//            set
//            {
//                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref term, value);
//            }
//        }
    }
}