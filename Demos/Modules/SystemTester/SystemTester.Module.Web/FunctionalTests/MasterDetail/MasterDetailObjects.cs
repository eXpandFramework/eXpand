using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.MasterDetail{
    [DefaultClassOptions]
    public class Master : BaseObject{
        public Master(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("ListViewAutoCommit-Details"), Aggregated]
        public XPCollection<Detail> Details{
            get { return GetCollection<Detail>("Details"); }
        }
    }

    public class Detail : BaseObject{
        private Master _master;

        public Detail(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("ListViewAutoCommit-Details")]
        public Master Master{
            get { return _master; }
            set { SetPropertyValue("ListViewAutoCommit", ref _master, value); }
        }
    }
}