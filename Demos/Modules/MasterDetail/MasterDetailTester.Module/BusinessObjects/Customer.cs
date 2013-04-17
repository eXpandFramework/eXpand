using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MasterDetailTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Customer : BaseObject {
        public Customer(Session session)
            : base(session) {
            Name = null;
        }

        public string Name { get; set; }
        [Association("Customer-Orders")]
        public XPCollection<Order> Orders {
            get { return GetCollection<Order>("Orders"); }
        }
    }

    public class Order : BaseObject {
        public Order(Session session)
            : base(session) {

        }
        [Association("Customer-Orders")]
        public Customer Customer { get; set; }

        public int Quantity { get; set; }
    }
}
