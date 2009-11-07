using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    public class PersistentInterfaceInfo:BaseObject, IInterfaceInfo {
        public PersistentInterfaceInfo(Session session) : base(session) {
        }
        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<PersistentClassInfo> PersistentClassInfos
        {
            get
            {
                return GetCollection<PersistentClassInfo>("PersistentClassInfos");
            }
        }
        
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private string _assembly;
        public string Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                SetPropertyValue("Assembly", ref _assembly, value);
            }
        }
    }
}