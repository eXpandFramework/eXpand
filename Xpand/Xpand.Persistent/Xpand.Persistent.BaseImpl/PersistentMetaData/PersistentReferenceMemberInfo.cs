using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(IPersistentReferenceMemberInfo))]
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
            }
        }

        void IPersistentReferenceMemberInfo.SetReferenceTypeFullName(string value) {
            ReferenceTypeFullName = value;
            if (_referenceType == null && _referenceClassInfo == null && value != null)
            {
                IPersistentClassInfo classInfo = PersistentClassInfoQuery.Find(Session, value);
                if (classInfo != null)
                    _referenceClassInfo = (PersistentClassInfo)classInfo;
                else
                    _referenceType = ReflectionHelper.GetType(value.Substring(value.LastIndexOf(".") + 1));
            }
        }
        public PersistentClassInfo ReferenceClassInfo
        {
            get { return _referenceClassInfo; }
            set
            {
                SetPropertyValue("ReferenceClassInfo", ref _referenceClassInfo, value);
                if (!IsLoading && !IsSaving)
                {
                    _referenceTypeFullName = _referenceClassInfo != null
                                                 ? _referenceClassInfo.PersistentAssemblyInfo.Name + "." +
                                                   _referenceClassInfo.Name
                                                 : null;
                    _referenceType = null;
                }
            }
        }

        IPersistentClassInfo IPersistentReferenceMemberInfo.ReferenceClassInfo {
            get { return ReferenceClassInfo; }
            set { ReferenceClassInfo=value as PersistentClassInfo; }
        }


        RelationType IPersistentAssociatedMemberInfo.RelationType {
            get { return _autoGenerateOtherPartMember ? RelationType.OneToMany : RelationType.Undefined; }
            set { _autoGenerateOtherPartMember = value == RelationType.OneToMany ? true : false; }
        }
        #endregion
    }
}