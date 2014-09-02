using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.Xpo {
    [Serializable]
    [NonPersistent]
    public abstract class XpandCustomObject : XPCustomObject, ISupportChangedMembers {
        public const string ChangedPropertiesName = "ChangedProperties";
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else

        private Guid _oid = Guid.Empty;
        [Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
        public Guid Oid {
            get { return _oid; }
            set { _oid = value; }
        }
#endif
        private bool _isDefaultPropertyAttributeInit;
        private XPMemberInfo _defaultPropertyMemberInfo;

        protected override void OnSaving() {
            base.OnSaving();
            if (TrucateStrings)
                DoTrucateStrings();
            if (!(Session is NestedUnitOfWork) && Session.IsNewObject(this) && _oid == Guid.Empty) {
                _oid = XpoDefault.NewGuid();
            }
        }


        public override string ToString() {
            if (!_isDefaultPropertyAttributeInit) {
                var attrib = ClassInfo.FindAttributeInfo(typeof(DefaultPropertyAttribute)) as DefaultPropertyAttribute;
                if (attrib != null) {
                    _defaultPropertyMemberInfo = ClassInfo.FindMember(attrib.Name);
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


        public const string CancelTriggerObjectChangedName = "CancelTriggerObjectChanged";
        protected XpandCustomObject(Session session)
            : base(session) {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public bool IsNewObject {
            get { return Session.IsNewObject(this); }
        }

        [ValueConverter(typeof(NullValueConverter)), Persistent, Browsable(false)]
        [Size(1)]
        public HashSet<string> ChangedProperties { get; set; }

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) {
            if (!CancelTriggerObjectChanged)
                base.TriggerObjectChanged(args);
        }

        [Browsable(false)]
        [NonPersistent]
        [MemberDesignTimeVisibility(false)]
        public bool CancelTriggerObjectChanged { get; set; }

        [Browsable(false)]
        [NonPersistent]
        [MemberDesignTimeVisibility(false)]
        public bool TrucateStrings { get; set; }

        private void DoTrucateStrings() {
            foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties) {
                if (xpMemberInfo.MemberType == typeof(string)) {
                    var value = xpMemberInfo.GetValue(this) as string;
                    if (value != null) {
                        value = TruncateValue(xpMemberInfo, value);
                        xpMemberInfo.SetValue(this, value);
                    }
                }
            }
        }
        protected override void OnSaved() {
            base.OnSaved();
            ChangedMemberCollector.CollectOnSave(this);
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            ChangedMemberCollector.Collect(this, propertyName);
        }
        string TruncateValue(XPMemberInfo xpMemberInfo, string value) {
            if (xpMemberInfo.HasAttribute(typeof(SizeAttribute))) {
                int size = ((SizeAttribute)xpMemberInfo.GetAttributeInfo(typeof(SizeAttribute))).Size;
                if (size > -1 && value.Length > size)
                    value = value.Substring(0, size - 1);
            } else if (value.Length > 99)
                value = value.Substring(0, 99);
            return value;
        }

    }

}