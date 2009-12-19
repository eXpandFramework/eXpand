using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.Base.PersistentMetaData;
using System;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "ReferenceType,ReferenceClassInfo")]
    public class PersistentReferenceMemberInfo : PersistentMemberInfo, IPersistentReferenceMemberInfo {
        public PersistentReferenceMemberInfo(Session session) : base(session) { }


        public PersistentReferenceMemberInfo(Session session,PersistentAssociationAttribute persistentAssociationAttribute) : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }


        Type _referenceType;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ReferenceType
        {
            get { return _referenceType; }
            set {
                SetPropertyValue("ReferenceType", ref _referenceType, value);
                if (!IsLoading && !IsSaving) {
                    _referenceTypeFullName = _referenceType != null ? _referenceType.FullName : null;
                    _referenceClassInfo = null;
                }
            }
        }
        private PersistentClassInfo _referenceClassInfo;
        public PersistentClassInfo ReferenceClassInfo
        {
            get
            {
                return _referenceClassInfo;
            }
            set
            {
                SetPropertyValue("ReferenceClassInfo", ref _referenceClassInfo, value);
                if (!IsLoading&&!IsSaving) {
                    _referenceTypeFullName = _referenceClassInfo != null ? _referenceClassInfo.PersistentAssemblyInfo.Name + "." + _referenceClassInfo.Name : null;
                    _referenceType = null;
                }
            }
        }
        private string _referenceTypeFullName;
        [Browsable(false)]
        public string ReferenceTypeFullName
        {
            get
            {
                return _referenceTypeFullName;
            }
            set
            {
                SetPropertyValue("ReferenceTypeFullName", ref _referenceTypeFullName, value);
            }
        }
        private bool _autoGenerateOtherPartMember;
        public bool AutoGenerateOtherPartMember
        {
            get
            {
                return _autoGenerateOtherPartMember;
            }
            set
            {
                SetPropertyValue("AutoGenerateOtherPartMember", ref _autoGenerateOtherPartMember, value);
            }
        }

        RelationType IPersistentAssociatedMemberInfo.RelationType
        {
            get
            {
                return _autoGenerateOtherPartMember? RelationType.OneToMany:RelationType.Undefined;
            }
            set
            {
            }
        }
    }
}