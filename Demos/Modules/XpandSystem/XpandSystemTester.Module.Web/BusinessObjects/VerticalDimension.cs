using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    public class VerticalDimension : BaseObject{
        // Fields...
        private string _title;

        public VerticalDimension(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }
    }
}