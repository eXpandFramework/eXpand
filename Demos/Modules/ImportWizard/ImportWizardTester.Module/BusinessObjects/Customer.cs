using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ImportWizardTester.Module.BusinessObjects{
    [DefaultClassOptions]
    public class Customer : Person{
        // Fields...
        private GenderObject _gender;
        private string _importName;

        public Customer(Session session) : base(session){
        }

        public GenderObject Gender{
            get { return _gender; }
            set { SetPropertyValue("Gender", ref _gender, value); }
        }

        public string ImportName{
            get { return _importName; }
            set { SetPropertyValue("ImportName", ref _importName, value); }
        }
    }

//    [DefaultProperty("Gender")]
//    [DefaultClassOptions]
    public class GenderObject : BaseObject{
        private string _gender;

        public GenderObject(Session session) : base(session){
        }

        // Fields...


        public string Gender {
            get { return _gender; }
            set { SetPropertyValue("Gender", ref _gender, value); }
        }
    }
}