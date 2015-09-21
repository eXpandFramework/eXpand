using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace IOTester.Module.FunctionalTests.Export{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Key")]
    public class ExportObject : BaseObject{
        private string _doNotSerialize;
        private string _key;

        public ExportObject(Session session) : base(session){
        }

        public string Key{
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        public string DoNotSerialize{
            get { return _doNotSerialize; }
            set { SetPropertyValue("DoNotSerialize", ref _doNotSerialize, value); }
        }

        [Association]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<ExportAggregatedObject> AggregatedObjects
        {
            get
            {
                return GetCollection<ExportAggregatedObject>("AggregatedObjects");
            }
        }

    }
}