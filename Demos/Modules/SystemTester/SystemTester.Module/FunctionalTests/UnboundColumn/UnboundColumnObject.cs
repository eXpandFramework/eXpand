using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.UnboundColumn{
    [DefaultClassOptions]
    public class UnboundColumnObject : BaseObject{
        

        public UnboundColumnObject(Session session) : base(session){
        }


        decimal _pay;

        public decimal Pay{
            get{ return _pay; }
            set{ SetPropertyValue(nameof(Pay), ref _pay, value); }
        }
    }
}