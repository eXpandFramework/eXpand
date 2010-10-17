using System.ComponentModel;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering {
    public class RFCategory : BaseObject,ITreeNode {
        public RFCategory(Session session) : base(session) {
        }
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("RFCategory-RFCustomers")]
        public XPCollection<RFCustomer> Customers {
            get { return GetCollection<RFCustomer>("Customers"); }
        }
        string ITreeNode.Name {
            get { return Name; }
        }
        private RFCategory _parent;

        [Association("RFCategory-Categorys")]
        public RFCategory Parent {
            get { return _parent; }
            set { SetPropertyValue("Parent", ref _parent, value); }
        }

        [Association("RFCategory-Categorys")]
        public XPCollection<RFCategory> Children {
            get { return GetCollection<RFCategory>("Children"); }
        }
        ITreeNode ITreeNode.Parent {
            get { return Parent; }
        }

        IBindingList ITreeNode.Children {
            get { return Children; }
        }

        
    }
}