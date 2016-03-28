using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.RichEdit{
    [XpandNavigationItem("PropertyEditors/RichEdit")]
    public class RichEditObject : BaseObject{
        private string _text;

        public RichEditObject(Session session) : base(session){
        }
        [Size(SizeAttribute.Unlimited)]
        public string Text{
            get { return _text; }
            set { SetPropertyValue("Text", ref _text, value); }
        }
    }
}