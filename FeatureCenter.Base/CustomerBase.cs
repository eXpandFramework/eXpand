using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Base {
    [NonPersistent]
    public abstract class CustomerBase : BaseObject, ICustomer {
        string _city;
        string _description;
        string _name;

        protected CustomerBase(Session session) :
            base(session) {
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return _description; }
            set { SetPropertyValue("Description", ref _description, value); }
        }
        
        [ImmediatePostData]
        [VisibleInListView(true)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [ImmediatePostData]
        [VisibleInListView(true)]
        public string City {
            get { return _city; }
            set { SetPropertyValue("City", ref _city, value); }
        }

        
    }
}