using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    public class Master : BaseObject{
        // Fields...
        private string _title;

        public Master(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }

        [Association("Master-Details"), Aggregated]
        public XPCollection<Detail> Details{
            get { return GetCollection<Detail>("Details"); }
        }
    }
}