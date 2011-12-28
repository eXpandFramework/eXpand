using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData {
    public class XpandCodeMemberInfo : XpandCustomMemberInfo {
        readonly Func<XPBaseObject, object> _callBack;

        public XpandCodeMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic, Func<XPBaseObject, object> callBack)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic, true) {
                _callBack = callBack;
        }

        public override object GetValue(object theObject) {
            var xpBaseObject = ((XPBaseObject)theObject);
            return !xpBaseObject.Session.IsObjectsLoading && !xpBaseObject.Session.IsObjectsSaving
                       ? _callBack(xpBaseObject)
                       : base.GetValue(theObject);
        }

        protected override bool CanPersist {
            get { return false; }
        }
    }
}
