using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.RichEdit{
    [XpandNavigationItem("PropertyEditors/RichEdit")]
    public class RichEditObject : BaseObject{
        private string _field;
        private string _field2;
        private string _text;

        public RichEditObject(Session session) : base(session){
        }


        public string Field{
            get { return _field; }
            set { SetPropertyValue("Field", ref _field, value); }
        }


        public string Field2{
            get { return _field2; }
            set { SetPropertyValue("Field2", ref _field2, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Text{
            get { return _text; }
            set { SetPropertyValue("Text", ref _text, value); }
        }
    }
}