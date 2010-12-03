using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.Persistent.Base {
    [Serializable]
    [NonPersistent]
    public abstract class XpandBaseCustomObject : Xpo.XpandCustomObject {
        private bool _isDefaultPropertyAttributeInit;
        private XPMemberInfo _defaultPropertyMemberInfo;
        protected XpandBaseCustomObject(Session session) : base(session) {
        }
        public override string ToString() {
            if (!_isDefaultPropertyAttributeInit) {
                string defaultPropertyName = string.Empty;
                var xafDefaultPropertyAttribute = XafTypesInfo.Instance.FindTypeInfo(GetType()).FindAttribute<XafDefaultPropertyAttribute>();
                if (xafDefaultPropertyAttribute != null) {
                    defaultPropertyName = xafDefaultPropertyAttribute.Name;
                } else {
                    var defaultPropertyAttribute = XafTypesInfo.Instance.FindTypeInfo(GetType()).FindAttribute<DefaultPropertyAttribute>();
                    if (defaultPropertyAttribute != null) {
                        defaultPropertyName = defaultPropertyAttribute.Name;
                    }
                }
                if (!string.IsNullOrEmpty(defaultPropertyName)) {
                    _defaultPropertyMemberInfo = ClassInfo.FindMember(defaultPropertyName);
                }
                _isDefaultPropertyAttributeInit = true;
            }
            if (_defaultPropertyMemberInfo != null) {
                object obj = _defaultPropertyMemberInfo.GetValue(this);
                if (obj != null) {
                    return obj.ToString();
                }
            }
            return base.ToString();

        }
    }
}
