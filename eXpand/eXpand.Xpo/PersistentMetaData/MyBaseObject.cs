using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    [NonPersistent]   
    public class MyBaseObject : XPObject {
        public MyBaseObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
    }
}