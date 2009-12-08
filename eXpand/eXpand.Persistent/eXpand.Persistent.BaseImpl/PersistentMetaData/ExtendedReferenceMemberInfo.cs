using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
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