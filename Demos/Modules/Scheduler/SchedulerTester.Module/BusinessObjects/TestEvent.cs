using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Scheduler.Reminders;

namespace SchedulerTester.Module.BusinessObjects {
    
    public class Customer:Person {
        public Customer(Session session) : base(session) {
        }
        [Association("Customer-TestEvents")]
        public XPCollection<TestEvent> TestEvents {
            get {
                return GetCollection<TestEvent>("TestEvents");
            }
        }
    }

    [SupportsReminder(Criteria = "Customer is not null")]
    [DefaultClassOptions]
    public class TestEvent:Event {
        public TestEvent(Session session) : base(session) {
        }
        // Fields...
        private Customer _customer;

        [Association("Customer-TestEvents")]
        public Customer Customer {
            get {
                return _customer;
            }
            set {
                SetPropertyValue("Customer", ref _customer, value);
            }
        }
    }
}
