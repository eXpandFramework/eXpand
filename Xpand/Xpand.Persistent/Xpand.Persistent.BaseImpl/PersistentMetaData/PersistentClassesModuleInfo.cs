using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData{
    public class PersistentClassesModuleInfo:XPBaseObject,IModuleInfo{
        public PersistentClassesModuleInfo(Session session) : base(session){
        }
        [Key(true)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string AssemblyFileName { get; set; }
        public string Version { get; set; }
        public bool IsMain { get; set; }

        public override string ToString() {
            return !string.IsNullOrEmpty(Name) ? Name : base.ToString();
        }

    }
}