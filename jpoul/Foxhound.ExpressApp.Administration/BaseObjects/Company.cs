using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class Company : CompanyUnit, ITreeNode {
        

        public Company(Session session) : base(session) {}

        [Association("Company-Units")]
        public XPCollection<CompanyUnit> Units{
            get { return GetCollection<CompanyUnit>("Units"); }
        }

        [Browsable(false)]
        ITreeNode ITreeNode.Parent {
            get { return null; }
        }

        [Browsable(false)]
        IBindingList ITreeNode.Children {
            get { return Units; }
        }
    }
}