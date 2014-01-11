using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.Base;
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
    public class Customer:Person{
        public Customer(Session session) : base(session){
        }
    }
}
