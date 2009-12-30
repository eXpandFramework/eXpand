using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "ReferenceType,ReferenceClassInfo")]
    public class PersistentReferenceMemberInfo : PersistentMemberInfo, IPersistentReferenceMemberInfo {
        bool _autoGenerateOtherPartMember;
        PersistentClassInfo _referenceClassInfo;
        Type _referenceType;
        string _referenceTypeFullName;

        public PersistentReferenceMemberInfo(Session session) : base(session) {
        }


        public PersistentReferenceMemberInfo(Session session,
                                             PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session) {
            TypeAttributes.Add(persistentAssociationAttribute);
        }


        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type ReferenceType {
            get { return _referenceType; }
            set {
                SetPropertyValue("ReferenceType", ref _referenceType, value);
                if (!IsLoading && !IsSaving) {
                    _referenceTypeFullName = _referenceType != null ? _referenceType.FullName : null;
                    _referenceClassInfo = null;
                }
            }
        }

        public PersistentClassInfo ReferenceClassInfo {
            get { return _referenceClassInfo; }
            set {
                SetPropertyValue("ReferenceClassInfo", ref _referenceClassInfo, value);
                if (!IsLoading && !IsSaving) {
                    _referenceTypeFullName = _referenceClassInfo != null
                                                 ? _referenceClassInfo.PersistentAssemblyInfo.Name + "." +
                                                   _referenceClassInfo.Name
                                                 : null;
                    _referenceType = null;
                }
            }
        }
        
        public bool AutoGenerateOtherPartMember {
            get { return _autoGenerateOtherPartMember; }
            set { SetPropertyValue("AutoGenerateOtherPartMember", ref _autoGenerateOtherPartMember, value); }
        }
        #region IPersistentReferenceMemberInfo Members
        [Browsable(false)]
        public string ReferenceTypeFullName {
            get { return _referenceTypeFullName; }
            set {
                SetPropertyValue("ReferenceTypeFullName", ref _referenceTypeFullName, value);
                if (!IsLoading&&!IsSaving) {
                    IPersistentClassInfo persistentClassInfo = PersistentClassInfoQuery.Find(Session, value);
                    if (persistentClassInfo != null)
                        _referenceClassInfo = (PersistentClassInfo) persistentClassInfo;
                    else
                        _referenceType = ReflectionHelper.GetType(value.Substring(value.LastIndexOf(".")+1)); 
                }
            }
        }

        RelationType IPersistentAssociatedMemberInfo.RelationType {
            get { return _autoGenerateOtherPartMember ? RelationType.OneToMany : RelationType.Undefined; }
            set { _autoGenerateOtherPartMember = value == RelationType.OneToMany ? true : false; }
        }
        #endregion
    }
}