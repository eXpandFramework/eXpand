using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ValidationTester.Module.FunctionalTests{
    [DefaultClassOptions]
    public class ValidationObject : BaseObject{
        private string _name;

        public ValidationObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}