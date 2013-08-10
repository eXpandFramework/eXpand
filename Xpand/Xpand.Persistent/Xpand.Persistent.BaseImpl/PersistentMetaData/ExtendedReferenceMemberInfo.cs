using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(IExtendedReferenceMemberInfo))]
    public class ExtendedReferenceMemberInfo:ExtendedMemberInfo,IExtendedReferenceMemberInfo {
        private Type _referenceType;

        public ExtendedReferenceMemberInfo(Session session) : base(session) {
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ReferenceType {
            get { return _referenceType; }
            set { SetPropertyValue("ReferenceType", ref _referenceType, value); }
        }
    }
}