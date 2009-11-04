using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    public class PersistentClassInfo : PersistentTypeInfo, IPersistentClassInfo {
        public const string DynamicAssemblyName = "WorldCreator";

        
        private PersistentClassInfo _baseClass;

        public PersistentClassInfo(Session session) : base(session) {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public PersistentClassInfo BaseClass {
            get { return _baseClass; }
            set { SetPropertyValue("BaseClass", ref _baseClass, value); }
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

        IPersistentClassInfo IPersistentClassInfo.BaseClass {
            get { return BaseClass; }
            set { BaseClass = value as PersistentClassInfo; }
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
                    Name = memberName,ReferenceType =Session.Dictionary.GetClassInfo("",string.Format("{0}.{1}",referenceType.AssemblyName,referenceType.Name)).ClassType
                                                                                                  };
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