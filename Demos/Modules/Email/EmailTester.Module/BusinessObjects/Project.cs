using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace EmailTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Project : BaseObject {
        // Fields...
        string _name;

        public Project(Session session) : base(session) {
        }

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("Project-Members"), AggregatedAttribute]
        public XPCollection<Member> Members {
            get { return GetCollection<Member>("Members"); }
        }
    }

    public class Member : BaseObject {
        // Fields...
        string _email;
        Project _project;

        public Member(Session session) : base(session) {
        }

        public string Email {
            get { return _email; }
            set { SetPropertyValue("Email", ref _email, value); }
        }

        [Association("Project-Members")]
        public Project Project {
            get { return _project; }
            set { SetPropertyValue("Project", ref _project, value); }
        }
    }
}