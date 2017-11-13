using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.BusinessObjects {
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Title")]
    public class Position : BaseObject {
        public Position(Session session)
            : base(session) {
        }
        private string title;
        [RuleRequiredField("RuleRequiredField for Position.Title", DefaultContexts.Save)]
        public string Title {
            get {
                return title;
            }
            set {
                SetPropertyValue("Title", ref title, value);
            }
        }
        [Association("Departments-Positions")]
        public XPCollection<Department> Departments {
            get {
                return GetCollection<Department>("Departments");
            }
        }
    }
}
