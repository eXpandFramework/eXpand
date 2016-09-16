using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.ModelAttributeValue {
    [DefaultClassOptions]
    public class ModelAttributeValueObject:BaseObject {
        public ModelAttributeValueObject(Session session) : base(session){
        }

        string _name;

        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }
    }

}
