using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.WorldCreator.Services {
    public class ExistentTypesMemberCreator {
        private static XPCustomMemberInfo[] _members= new XPCustomMemberInfo[0];

        static XPCustomMemberInfo[] CreateMembers(IObjectSpace objectSpace) {
            var members =CreateCollectionMembers(objectSpace)
                    .Concat(CreateReferenceMembers(objectSpace).Concat(CreateCoreMembers(objectSpace)))
                    .ToArray();

            foreach (var type in members.Select(info => info.Owner.ClassType).Distinct()) {
                XafTypesInfo.Instance.RefreshInfo(type);
            }
            return members;
        }

        static IEnumerable<IExtendedMemberInfo> GetMembers(IObjectSpace objectSpace, Type infoType) {
            return objectSpace.GetObjects(infoType).Cast<IExtendedMemberInfo>().Where(info => !MemberExists(info,objectSpace));
        }

        private static bool MemberExists(IExtendedMemberInfo extendedMemberInfo, IObjectSpace objectSpace) {
            var typeInfo = objectSpace.TypesInfo.FindTypeInfo(extendedMemberInfo.Owner);
            return typeInfo?.FindMember(extendedMemberInfo.Name) != null;
        }

        static IEnumerable<XPCustomMemberInfo> CreateCollectionMembers(IObjectSpace objectSpace) {
            IEnumerable<IExtendedMemberInfo> xpCollection = GetMembers(objectSpace, XafTypesInfo.Instance.FindBussinessObjectType<IExtendedCollectionMemberInfo>());
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            foreach (var info in collection) {
                XPCustomMemberInfo member = GetXPCustomMemberInfo(info);
                if (member != null) {
                    CreateAttributes(info, member);
                    yield return member;
                }
            }
        }

        static XPCustomMemberInfo GetXPCustomMemberInfo(IExtendedCollectionMemberInfo info) {
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

        static IEnumerable<XPCustomMemberInfo> CreateReferenceMembers(IObjectSpace objectSpace) {
            var xpCollection = GetMembers(objectSpace, XafTypesInfo.Instance.FindBussinessObjectType<IExtendedReferenceMemberInfo>());
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()) {
                var referenceType = info.ReferenceType;
                var member = GetMember(info, referenceType);
                if (member != null) {
                    CreateAttributes(info, member);
	                yield return member;
                }
            }
        }

        static IEnumerable<XPCustomMemberInfo> CreateCoreMembers(IObjectSpace objectSpace) {
            var memberInfos = GetMembers(objectSpace, XafTypesInfo.Instance.FindBussinessObjectType<IExtendedCoreTypeMemberInfo>());
            foreach (var info in memberInfos.Cast<IExtendedCoreTypeMemberInfo>()) {
                var referenceType = Type.GetType("System." + info.DataType, true);
                var member = GetMember(info, referenceType);
                if (member != null) {
                    CreateAttributes(info, member);
                    yield return member;
                }
            }
        }

        static XPCustomMemberInfo GetMember(IExtendedMemberInfo info, Type referenceType) {
            if (info.Owner != null) {
                var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(info.Owner);
                return info.TypeAttributes.OfType<IPersistentPersistentAliasAttribute>().FirstOrDefault() == null
                           ? classInfo.CreateMember(info.Name, referenceType)
                           : classInfo.CreateCalculabeMember(info.Name, referenceType, "");
            }
            return null;
        }

        static void CreateAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo memberInfo) {
            foreach (AttributeInfoAttribute attributeInfo in extendedMemberInfo.TypeAttributes.Select(typeAttribute => typeAttribute.Create())) {
                memberInfo.AddAttribute((Attribute)ReflectionHelper.CreateObject(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues));
            }
        }

        public static XPCustomMemberInfo[] CreateMembers(WorldCreatorModuleBase worldCreatorModule){
            if (_members.Length==0  && InterfaceBuilder.RuntimeMode){
                using (var worldCreatorObjectSpaceProvider = new WorldCreatorObjectSpaceProvider()){
                    using (var objectSpace = worldCreatorObjectSpaceProvider.CreateObjectSpace()){
                        _members = CreateMembers(objectSpace);
                    }
                }

                worldCreatorModule.Application.ObjectSpaceCreated += ApplicationOnObjectSpaceCreated;
            }
            return _members;
        }

        private static void ApplicationOnObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e){
            var application = ((XafApplication) sender);
            application.ObjectSpaceCreated-=ApplicationOnObjectSpaceCreated;
            var memberInfoGroups = _members.Where(info => info.IsPersistent).GroupBy(info => info.MemberType);
            foreach (var memberInfoGroup in memberInfoGroups){
                using (var objectSpace = application.CreateObjectSpace(memberInfoGroup.Key)){
                    foreach (var info in memberInfoGroup){
                        objectSpace.CreateColumn(info);
                    }
                }
            }
        }
    }
}