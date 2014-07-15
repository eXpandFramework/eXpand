using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace WizardUITester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestClass:BaseObject {
        public TestClass(Session session) : base(session) {
        }
        // Fields...
        private string _PropertyName;
        private string _page2;
        private string _page1;

        public string Page1 {
            get { return _page1; }
            set {
                _page1 = value;
            }
        }
        [RuleRequiredField]
        public string Page2 {
            get { return _page2; }
            set {
                _page2 = value;
            }
        }

        
    }
}
