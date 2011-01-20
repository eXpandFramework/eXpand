using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl {
    [InterfaceRegistrator(typeof(ISequenceObject))]
    public class XpandSequence : XPBaseObject, ISequenceObject {
        private string typeName;
        private long nextSequence;
        public XpandSequence(Session session)
            : base(session) {
        }
        [Key]
        [Size(1024)]
        public string TypeName {
            get { return typeName; }
            set { SetPropertyValue("TypeName", ref typeName, value); }
        }
        public long NextSequence {
            get { return nextSequence; }
            set { SetPropertyValue("NextSequence", ref nextSequence, value); }
        }
    }
}