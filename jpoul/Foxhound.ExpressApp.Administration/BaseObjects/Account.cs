using System.Reflection;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace Foxhound.ExpressApp.Administration.BaseObjects
{
    [DefaultClassOptions]
    public class Account : TaxonomyBaseObject
    {
        public Account(Session session) : base(session) { }

        private Employee employee;

        [ProvidedAssociation(Associations.EmployeeAccounts)]
        [Association(Associations.EmployeeAccounts)]
        public Employee Manager
        {
            get
            {
                return employee;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref employee, value);
            }
        }
    }

}
