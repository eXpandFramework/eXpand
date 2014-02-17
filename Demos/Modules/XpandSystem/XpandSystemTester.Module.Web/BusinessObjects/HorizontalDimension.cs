using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    public class HorizontalDimension : BaseObject{
        // Fields...
        private string _title;

        public HorizontalDimension(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }
    }
}