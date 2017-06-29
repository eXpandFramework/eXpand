using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.TrackModifiedProperties{
    
    [DefaultClassOptions]
    public class TrackModifiedPropertiesObject : BaseObject{
        
        private string _name;

        public TrackModifiedPropertiesObject(Session session) : base(session){
        }

        string _name2;

        public string Name2{
            get{ return _name2; }
            set{ SetPropertyValue(nameof(Name2), ref _name2, value); }
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}