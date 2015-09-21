using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;

namespace IOTester.Module.FunctionalTests.Export
{
    [DefaultClassOptions]
    public class ExportAggregatedObject : BaseObject
    {
        private string _nestedKey;
        private IOObject _parentIOObject;

        public ExportAggregatedObject(Session session) : base(session)
        {
        }
                
        [Association]
        [SerializationKey]
        public IOObject ParentIOObject
        {
            get { return _parentIOObject; }
            set { SetPropertyValue("ParentObject", ref _parentIOObject, value); }
        }
        
        public string NestedKey
        {
            get { return _nestedKey; }
            set { SetPropertyValue("NestedKey", ref _nestedKey, value); }
        }
        
    }
}