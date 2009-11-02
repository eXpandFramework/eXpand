using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
//    [DefaultClassOptions]
//    [NonPersistent]
    public class InterfaceInfo:BaseObject, IInterfaceInfo {
        public InterfaceInfo(Session session) : base(session) {
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