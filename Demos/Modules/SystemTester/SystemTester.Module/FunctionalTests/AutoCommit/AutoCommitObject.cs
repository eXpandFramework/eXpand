using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.AutoCommit{
    [DefaultClassOptions]
    public class AutoCommitObject : BaseObject{
        private bool _check;

        private string _name;

        public AutoCommitObject(Session session) : base(session){
        }

        [Index(1)]
        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [Index(0)]
        public bool Check{
            get { return _check; }
            set { SetPropertyValue(nameof(Check), ref _check, value); }
        }

        [Action]
        public void AutoCommit(){
        }
    }
}