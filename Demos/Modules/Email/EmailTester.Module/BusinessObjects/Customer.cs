using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace EmailTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Customer:Person {
        public Customer(Session session) : base(session) {
        }
    }

    [DefaultClassOptions]
    public class EmailTask:Task{
        public EmailTask(Session session) : base(session){
        }
    }
}
