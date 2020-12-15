using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData{
    [InterfaceRegistrator(typeof(IPersistentReferenceMemberInfo))]
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "ReferenceType,ReferenceClassInfo")]
    [System.ComponentModel.DisplayName("Reference")]
    public class PersistentReferenceMemberInfo : PersistentMemberInfo, IPersistentReferenceMemberInfo{
        bool _autoGenerateOtherPartMember;
        PersistentClassInfo _referenceClassInfo;
        Type _referenceType;
        string _referenceTypeFullName;

        public PersistentReferenceMemberInfo(Session session) : base(session){
        }


        public PersistentReferenceMemberInfo(Session session,
            PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session){
            TypeAttributes.Add(persistentAssociationAttribute);
        }


        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ReferenceType{
            get => _referenceType;
            set{
                SetPropertyValue("ReferenceType", ref _referenceType, value);
                if (!IsLoading && !IsSaving){
                    _referenceTypeFullName = _referenceType?.FullName;
                    _referenceClassInfo = null;
                }
            }
        }


        public bool AutoGenerateOtherPartMember{
            get => _autoGenerateOtherPartMember;
            set => SetPropertyValue("AutoGenerateOtherPartMember", ref _autoGenerateOtherPartMember, value);
        }

        #region IPersistentReferenceMemberInfo Members

        [Browsable(false)]
        public string ReferenceTypeFullName{
            get => _referenceTypeFullName;
            set => SetPropertyValue("ReferenceTypeFullName", ref _referenceTypeFullName, value);
        }

        void IPersistentReferenceMemberInfo.SetReferenceTypeFullName(string value){
            ReferenceTypeFullName = value;
            if (_referenceType == null && _referenceClassInfo == null && value != null){
                IPersistentClassInfo classInfo = Session.FindReferenceClassInfo( value);
                if (classInfo != null)
                    _referenceClassInfo = (PersistentClassInfo) classInfo;
                else
                    _referenceType =
                        ReflectionHelper.GetType(value.Substring(value.LastIndexOf(".", StringComparison.Ordinal) + 1));
            }
        }

        public PersistentClassInfo ReferenceClassInfo{
            get => _referenceClassInfo;
            set{
                SetPropertyValue("ReferenceClassInfo", ref _referenceClassInfo, value);
                if (!IsLoading && !IsSaving){
                    _referenceTypeFullName = _referenceClassInfo?.PersistentAssemblyInfo != null
                        ? _referenceClassInfo.PersistentAssemblyInfo.Name + "." +
                          _referenceClassInfo.Name
                        : null;
                    _referenceType = null;
                }
            }
        }

        IPersistentClassInfo IPersistentReferenceMemberInfo.ReferenceClassInfo{
            get => ReferenceClassInfo;
            set => ReferenceClassInfo = value as PersistentClassInfo;
        }

        #endregion
    }
}