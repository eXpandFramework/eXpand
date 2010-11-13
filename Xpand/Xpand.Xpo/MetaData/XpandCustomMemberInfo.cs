using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData {
    public class XpandCustomMemberInfo : XPCustomMemberInfo {
        public XpandCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic) {
        }        
    }

}