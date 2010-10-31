using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(IExtendedOrphanedCollection))]
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "ElementClassInfo,ElementType")]
    public class ExtendedOrphanedCollection : ExtendedCollectionMemberInfo, IExtendedOrphanedCollection {
        public ExtendedOrphanedCollection(Session session) : base(session) {
        }
        private string _criteria;
        Type _elementType;
        string _elementTypeFullName;
        PersistentClassInfo _elementClassInfo;

        public string Criteria {
            get {
                return _criteria;
            }
            set {
                SetPropertyValue("Criteria", ref _criteria, value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ElementType {
            get { return _elementType; }
            set {
                SetPropertyValue("ElementType", ref _elementType, value);
                if (_elementType != null)
                    _elementTypeFullName = _elementType.FullName;
                else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        public PersistentClassInfo ElementClassInfo {
            get { return _elementClassInfo; }
            set {
                SetPropertyValue("ElementClassInfo", ref _elementClassInfo, value);
                if (_elementClassInfo != null && _elementClassInfo.PersistentAssemblyInfo != null) {
                    _elementTypeFullName = _elementClassInfo.PersistentAssemblyInfo.Name + "." + _elementClassInfo.Name;
                } else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string ElementTypeFullName {
            get { return _elementTypeFullName; }
            set { SetPropertyValue("ElementTypeFullName", ref _elementTypeFullName, value); }
        }
    }
}