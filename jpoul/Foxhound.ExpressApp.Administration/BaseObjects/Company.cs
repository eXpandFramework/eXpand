using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;
using System;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class Company : Organization {
        

        public Company(Session session) : base(session) {}
        [Association(Associations.CompanyCompanyUnits)]
        public XPCollection<CompanyUnit> CompanyUnits
        {
            get
            {
                return GetCollection<CompanyUnit>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }

        //[Association("Company-Units")]
        //public XPCollection<CompanyUnit> Units{
        //    get { return GetCollection<CompanyUnit>("Units"); }
        //}

        //[Browsable(false)]
        //ITreeNode ITreeNode.Parent {
        //    get { return null; }
        //}

        //[Browsable(false)]
        //IBindingList ITreeNode.Children {
        //    get { return Units; }
        //}
    }
}