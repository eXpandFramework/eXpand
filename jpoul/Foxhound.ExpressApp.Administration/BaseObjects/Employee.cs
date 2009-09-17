using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using System;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    
    public class Employee : TaxonomyBaseObject {
        private Term companyUnit;
        public Employee(Session session) : base(session) { }
        
        public Term CompanyUnit
        {
            get
            {
                return companyUnit;
            }
            set
            {
                SetPropertyValue("CompanyUnit", ref companyUnit, value);
            }
            
        }
        
        //public Term Company
        //{
        //    get
        //    {
        //        if ((CompanyUnit!= null)){
        //            return CompanyUnit.ParentTerm;
        //        }
        //        return null;
        //    }
        //}

    }
}
