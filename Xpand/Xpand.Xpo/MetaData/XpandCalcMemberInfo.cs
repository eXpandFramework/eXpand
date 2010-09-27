using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData {
    public class XpandCalcMemberInfo:XPCustomMemberInfo {
        readonly string _propertyName;

        public XpandCalcMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, XPClassInfo referenceType, bool nonPersistent, bool nonPublic) : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic) {
            _propertyName = propertyName;
        }
        public override object GetValue(object theObject) {
            var xpBaseObject = ((XPBaseObject)theObject);
            if (!xpBaseObject.Session.IsObjectsLoading&&!xpBaseObject.Session.IsObjectsSaving)
                return xpBaseObject.EvaluateAlias(_propertyName);
            return base.GetValue(theObject);
        }

        protected override bool CanPersist {
            get { return false; }
        }
    }
}
