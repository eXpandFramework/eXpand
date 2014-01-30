using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace TreeListEditorsTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class MyCategory:HCategory {
        public MyCategory(Session session) : base(session){
        }

        public MyCategory(Session session, string name) : base(session, name){
        }

    }
    [DefaultClassOptions]
    public class Customer:Person,ICategorizedItem{
        public Customer(Session session) : base(session){
        }

        ITreeNode ICategorizedItem.Category { get; set; }
        // Fields...
        private MyCategory _category;

        public MyCategory Category {
            get {
                return _category;
            }
            set {
                SetPropertyValue("Category", ref _category, value);
            }
        }
    }
}
