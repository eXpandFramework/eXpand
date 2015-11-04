using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace IOTester.Module.FunctionalTests.SimpleIO{
    [DefaultClassOptions]
    [DefaultProperty("Key")]
    public class SimpleIO : BaseObject {
        private string _doNotSerialize;
        private string _key;

        public SimpleIO(Session session)
            : base(session) {
        }

        public string Key {
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        public string DoNotSerialize {
            get { return _doNotSerialize; }
            set { SetPropertyValue("DoNotSerialize", ref _doNotSerialize, value); }
        }
    }
}