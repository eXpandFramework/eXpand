using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;

namespace IOTester.Module.FunctionalTests
{
    [DefaultClassOptions]
    public class IOAggregatedObject : BaseObject
    {
        private string _nestedKey;
        private IOObject _parentObject;

        public IOAggregatedObject(Session session) : base(session)
        {
        }
                
        [Association]
        [SerializationKey]
        public IOObject ParentObject
        {
            get { return _parentObject; }
            set { SetPropertyValue("ParentObject", ref _parentObject, value); }
        }
        
        public string NestedKey
        {
            get { return _nestedKey; }
            set { SetPropertyValue("NestedKey", ref _nestedKey, value); }
        }
        
    }
}