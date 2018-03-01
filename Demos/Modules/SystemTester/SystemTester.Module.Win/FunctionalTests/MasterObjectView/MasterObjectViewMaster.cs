using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Win.FunctionalTests.MasterObjectView{
    [XpandNavigationItem("MasterObjectView/Master")]
    public class MasterObjectViewMaster : BaseObject{
        


        public MasterObjectViewMaster(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [DataSourceCriteria("MasterObjectViewMaster.Oid='@This.Oid'")]
        public XPCollection<MasterObjectViewNested> Nesteds => new XPCollection<MasterObjectViewNested>(Session);
    }

    [XpandNavigationItem("MasterObjectView/Nested")]
    public class MasterObjectViewNested : BaseObject {
        

        public MasterObjectViewNested(Session session) : base(session){
        }

        MasterObjectViewMaster _masterObjectViewMaster;

        
        public MasterObjectViewMaster MasterObjectViewMaster{
            get{ return _masterObjectViewMaster; }
            set{ SetPropertyValue(nameof(MasterObjectViewMaster), ref _masterObjectViewMaster, value); }
        }
        public string Name { get; set; }

        
    }
}