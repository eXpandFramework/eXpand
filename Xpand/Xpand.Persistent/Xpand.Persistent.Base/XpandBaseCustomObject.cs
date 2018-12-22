using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo;

namespace Xpand.Persistent.Base {
    
    [NonPersistent]
    public abstract class XpandBaseCustomObject : XpandCustomObject,IObjectSpaceLink {
        private bool _isDefaultPropertyAttributeInit;
        private XPMemberInfo _defaultPropertyMemberInfo;
        protected XpandBaseCustomObject(Session session) : base(session) {
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            Oid = XpoDefault.NewGuid();
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
            object obj = _defaultPropertyMemberInfo?.GetValue(this);
            return obj?.ToString() ?? base.ToString();
        }
        [Browsable(false)]
        public IObjectSpace ObjectSpace { get; set; }
    }
}
