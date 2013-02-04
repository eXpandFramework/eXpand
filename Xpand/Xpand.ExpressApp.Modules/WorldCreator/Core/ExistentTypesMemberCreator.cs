using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public class ExistentTypesMemberCreator {
        public void CreateMembers(Session session) {
            var types = CreateCollectionMembers(session);
            types.AddRange(CreateReferenceMembers(session));
            types.AddRange(CreateCoreMembers(session));

            foreach (var type in types) {
                XpandModuleBase.TypesInfo.RefreshInfo(type);
            }
        }

        public IEnumerable<IExtendedMemberInfo> GetMembers(Session session, Type infoType) {
            IEnumerable<IExtendedMemberInfo> extendedMemberInfos = new XPCollection(session, infoType).Cast<IExtendedMemberInfo>();
            return extendedMemberInfos.Where(info => !MemberExists(info));
        }

        private bool MemberExists(IExtendedMemberInfo info) {
            var typeInfo = XpandModuleBase.TypesInfo.FindTypeInfo(info.Owner);
            return typeInfo != null && typeInfo.FindMember(info.Name) != null;
        }

        public List<Type> CreateCollectionMembers(Session session) {
            IEnumerable<IExtendedMemberInfo> xpCollection = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCollectionMemberInfo>());
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            var types = new List<Type>();
            foreach (var info in collection) {
                XPCustomMemberInfo member = GetXPCustomMemberInfo(info);
                if (member != null) {
                    CreateAttributes(info, member);
                    types.Add(info.Owner);
                }
            }
            return types;
        }

        XPCustomMemberInfo GetXPCustomMemberInfo(IExtendedCollectionMemberInfo info) {
            if (info.Owner != null) {
                var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(info.Owner);
                if (!(info is IExtendedOrphanedCollection)) {
                    return classInfo.CreateMember(info.Name, typeof(XPCollection), true);
                }
                var extendedOrphanedCollection = ((IExtendedOrphanedCollection)info);
                return classInfo.CreateCollection(info.Name, ReflectionHelper.FindType(extendedOrphanedCollection.ElementTypeFullName), extendedOrphanedCollection.Criteria);
            }
            return null;
        }

        public List<Type> CreateReferenceMembers(Session session) {
            var types = new List<Type>();
            var xpCollection = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedReferenceMemberInfo>());
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()) {
                var referenceType = info.ReferenceType;
                var member = GetMember(info, referenceType);
                if (member != null) {
                    CreateAttributes(info, member);
                    types.Add(referenceType);
                }
            }
            return types;
        }

        public List<Type> CreateCoreMembers(Session session) {
            var types = new List<Type>();
            var memberInfos = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCoreTypeMemberInfo>());
            foreach (var info in memberInfos.Cast<IExtendedCoreTypeMemberInfo>()) {
                var referenceType = Type.GetType("System." + info.DataType, true);
                var member = GetMember(info, referenceType);
                if (member != null) {
                    CreateAttributes(info, member);
                    types.Add(info.Owner);
                }
            }
            return types;
        }

        XPCustomMemberInfo GetMember(IExtendedMemberInfo info, Type referenceType) {
            if (info.Owner != null) {
                var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(info.Owner);
                return info.TypeAttributes.OfType<IPersistentPersistentAliasAttribute>().FirstOrDefault() == null
                           ? classInfo.CreateMember(info.Name, referenceType)
                           : classInfo.CreateCalculabeMember(info.Name, referenceType, "");
            }
            return null;
        }

        public void CreateAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo memberInfo) {
            foreach (AttributeInfoAttribute attributeInfo in extendedMemberInfo.TypeAttributes.Select(typeAttribute => typeAttribute.Create())) {
                memberInfo.AddAttribute((Attribute)ReflectionHelper.CreateObject(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues));
            }
        }
    }
}