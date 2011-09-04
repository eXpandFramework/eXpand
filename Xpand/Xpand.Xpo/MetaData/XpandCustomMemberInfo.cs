using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData {
    public class XpandCustomMemberInfo : XPCustomMemberInfo {
        public XpandCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic)
            : this(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic, false) {
        }

        public XpandCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic, bool readOnly)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic) {
            if (readOnly)
                typeof(XPMemberInfo).GetField("isReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true);
        }
    }

}