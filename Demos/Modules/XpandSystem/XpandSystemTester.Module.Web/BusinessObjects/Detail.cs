using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    public class Detail : BaseObject{
        private Master _master;
        private string _title;

        public Detail(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }


        [Association("Master-Details")]
        public Master Master{
            get { return _master; }
            set { SetPropertyValue("Master", ref _master, value); }
        }
    }
}