using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base {
    public class XpandLocalizedClassInfoTypeConverter : LocalizedClassInfoTypeConverter {
        public XpandLocalizedClassInfoTypeConverter(){
            AllowAddNonPersistentObjects = true;
        }
    }
}