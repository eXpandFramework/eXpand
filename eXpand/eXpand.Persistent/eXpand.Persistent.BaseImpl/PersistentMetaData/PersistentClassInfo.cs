using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class PersistentClassInfo : PersistentTypeInfo, IPersistentClassInfo {
        public const string DynamicAssemblyName = "WorldCreator";

        
        private Type _baseType;

        public PersistentClassInfo(Session session) : base(session) {
        }


        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type BaseType {
            get { return _baseType; }
            set {
                SetPropertyValue("BaseType", ref _baseType, value);
                if (value != null) _baseTypeAssemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }
        private string _baseTypeAssemblyQualifiedName;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)][MemberDesignTimeVisibility(false)]
        public string BaseTypeAssemblyQualifiedName
        {
            get
            {
                return _baseTypeAssemblyQualifiedName;
            }
            set
            {
                SetPropertyValue("BaseTypeAssemblyQualifiedName", ref _baseTypeAssemblyQualifiedName, value);
            }
        }
        [Association]
        public XPCollection<PersistentMemberInfo> OwnMembers {
            get { return GetCollection<PersistentMemberInfo>("OwnMembers"); }
        }

        #region IPersistentClassInfo Members
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string AssemblyName {
            get { return DynamicAssemblyName; }
        }

        [MemberDesignTimeVisibility(false)]
        [Browsable(false)]
        public XPClassInfo PersistentTypeClassInfo {
            get { return Session.Dictionary.GetClassInfo("", string.Format("{0}.{1}", AssemblyName, Name)); }
        }

        public virtual Type GetDefaultBaseClass() {
            return typeof (eXpandCustomObject);
        }

        IList<IPersistentMemberInfo> IPersistentClassInfo.OwnMembers {
            get { return new ListConverter<IPersistentMemberInfo, PersistentMemberInfo>(OwnMembers); }
        }

        Type IPersistentClassInfo.BaseType {
            get { return BaseType; }
            set { BaseType = value ; }
        }
        #endregion
        public PersistentReferenceMemberInfo AddReferenceMemberInfo(PersistentClassInfo referenceType) {
            return AddReferenceMemberInfo(referenceType.Name, referenceType);
        }

        public PersistentReferenceMemberInfo AddReferenceMemberInfo(string memberName, PersistentClassInfo referenceType) {
            return AddReferenceMemberInfo(memberName, memberName, referenceType);
        }

        public PersistentReferenceMemberInfo AddReferenceMemberInfo(string associationName, string memberName,
                                                                    PersistentClassInfo referenceType) {
            var persistentReferenceMemberInfo = 
                new PersistentReferenceMemberInfo(Session,
                    new PersistentAssociationAttribute(Session) {
                                                                    AssociationName =associationName
                                                                }) {
                    Name = memberName,ReferenceType =referenceType.PersistentTypeClassInfo.ClassType};
            OwnMembers.Add(persistentReferenceMemberInfo);
            return persistentReferenceMemberInfo;
        }

        public PersistentCollectionMemberInfo AddCollectionMemberInfo(Type elementType, string memberName) {
            var info = new PersistentCollectionMemberInfo(Session,
                                                          new PersistentAssociationAttribute(Session)
                                                          {ElementType = elementType}) {Name = memberName};
            OwnMembers.Add(info);
            return info;
        }

        public PersistentCollectionMemberInfo AddCollectionMemberInfo(PersistentClassInfo persistentClassInfo,
                                                                      Type elementType) {
            return AddCollectionMemberInfo(elementType, persistentClassInfo.Name);
        }
    }
}