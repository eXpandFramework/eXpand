using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData{
    [InterfaceRegistrator(typeof(IPersistentCollectionMemberInfo))]
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "CollectionType,CollectionClassInfo")]
    [System.ComponentModel.DisplayName("Collection")]
    public class PersistentCollectionMemberInfo : PersistentMemberInfo, IPersistentCollectionMemberInfo{
        private PersistentClassInfo _collectionClassInfo;
        private Type _collectionType;
        private string _collectionTypeFullName;

        public PersistentCollectionMemberInfo(Session session) : base(session){
        }

        public PersistentCollectionMemberInfo(PersistentAssociationAttribute persistentAssociationAttribute)
            : this(Session.DefaultSession, persistentAssociationAttribute){
        }

        public PersistentCollectionMemberInfo(Session session,
            PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session){
            TypeAttributes.Add(persistentAssociationAttribute);
        }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type CollectionType{
            get { return _collectionType; }
            set{
                SetPropertyValue("CollectionType", ref _collectionType, value);
                if (!IsLoading && !IsSaving){
                    _collectionTypeFullName = _collectionType?.FullName;
                    _collectionClassInfo = null;
                }
            }
        }

        #region IPersistentCollectionMemberInfo Members

        [Browsable(false)]
        public string CollectionTypeFullName{
            get { return _collectionTypeFullName; }
            set { SetPropertyValue("CollectionTypeFullName", ref _collectionTypeFullName, value); }
        }

        void IPersistentCollectionMemberInfo.SetCollectionTypeFullName(string s){
            CollectionTypeFullName = s;
            if (_collectionType == null && _collectionClassInfo == null && s != null){
                var classInfo = Session.FindReferenceClassInfo(s);
                if (classInfo != null)
                    _collectionClassInfo = (PersistentClassInfo) classInfo;
                else
                    _collectionType =
                        ReflectionHelper.GetType(s.Substring(s.LastIndexOf(".", StringComparison.Ordinal) + 1));
            }
        }

        public PersistentClassInfo CollectionClassInfo{
            get { return _collectionClassInfo; }
            set{
                SetPropertyValue("CollectionClassInfo", ref _collectionClassInfo, value);
                if (!IsLoading && !IsSaving){
                    _collectionTypeFullName = _collectionClassInfo?.PersistentAssemblyInfo != null
                        ? _collectionClassInfo.PersistentAssemblyInfo.Name + "." +
                          _collectionClassInfo.Name
                        : null;
                    _collectionType = null;
                }
            }
        }

        IPersistentClassInfo IPersistentCollectionMemberInfo.CollectionClassInfo{
            get { return CollectionClassInfo; }
            set { CollectionClassInfo = value as PersistentClassInfo; }
        }

        #endregion
    }
}