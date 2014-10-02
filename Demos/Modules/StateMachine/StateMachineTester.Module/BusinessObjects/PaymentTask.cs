using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace StateMachineTester.Module.BusinessObjects{
    [DefaultClassOptions]
    [ImageName("BO_Invoice")]
    public class PaymentTask : Task {

        public override void AfterConstruction() {
            base.AfterConstruction();
            PaymentStatus = Session.FindObject<Status>(new BinaryOperator("Caption", "New"));
        }

        public PaymentTask(Session session)
            : base(session) {

        }
        public override string ToString() {
            return Subject;
        }
        public Status PaymentStatus {
            get { return GetPropertyValue<Status>("PaymentStatus"); }
            set { SetPropertyValue("PaymentStatus", value); }
        }
        public BillStatus BillStatus {
            get { return GetPropertyValue<BillStatus>("BillStatus"); }
            set { SetPropertyValue("BillStatus", value); }
        }
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [FileTypeFilter("AllFiles", 2, "*.*")]
        public FileData Bill {
            get { return GetPropertyValue<FileData>("Bill"); }
            set { SetPropertyValue("Bill", value); }
        }
    }

    [DefaultProperty("Caption")]
    [NavigationItem("State Machine")]
    [ImageName("BO_List")]
    public class Status : BaseObject {
        private string _caption;
        public Status(Session session)
            : base(session) {

        }
        public string Caption {
            get { return _caption; }
            set { SetPropertyValue("Caption", ref _caption, value); }
        }
    }

}