using System;
using DevExpress.Xpo.Metadata;
using Fasterflect;

namespace Xpand.Xpo.MetaData {
    public class XpandCustomMemberInfo : XPCustomMemberInfo {
        public XpandCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic)
            : this(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic, false) {
        }

        public XpandCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic, bool readOnly)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic) {
            if (readOnly)
                this.SetFieldValue("isReadOnly", true);
        }
    }

}