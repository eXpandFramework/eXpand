using System.ComponentModel;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.Miscellaneous.RecursiveFiltering {
    public class Category : BaseObject,ITreeNode {
        public Category(Session session) : base(session) {
        }
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("Category-RFCustomers")]
        public XPCollection<RFCustomer> Customers {
            get { return GetCollection<RFCustomer>("Customers"); }
        }
        string ITreeNode.Name {
            get { return Name; }
        }
        private Category _parent;

        [Association("Category-Categorys")]
        public Category Parent {
            get { return _parent; }
            set { SetPropertyValue("Parent", ref _parent, value); }
        }

        [Association("Category-Categorys")]
        public XPCollection<Category> Children {
            get { return GetCollection<Category>("Children"); }
        }
        ITreeNode ITreeNode.Parent {
            get { return Parent; }
        }

        IBindingList ITreeNode.Children {
            get { return Children; }
        }

        
    }
}