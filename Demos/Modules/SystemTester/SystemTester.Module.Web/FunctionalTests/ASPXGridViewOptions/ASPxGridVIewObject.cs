using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.ASPXGridViewOptions {
    [DefaultClassOptions]
    public class ASPxGridVIewObject:BaseObject {
        string _stringProperty;

        public ASPxGridVIewObject(Session session) : base(session){
        }

        public string StringProperty{
            get { return _stringProperty; }
            set { SetPropertyValue(nameof(StringProperty), ref _stringProperty, value); }
        }

        string _name;

        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        bool _boolProperty;

        public bool BoolProperty{
            get { return _boolProperty; }
            set { SetPropertyValue(nameof(BoolProperty), ref _boolProperty, value); }
        }
    }
}
