using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class ExistentTypesMemberCreator
    {
        public void CreateMembers(Session session, TypesInfo typesInfo,XPDictionary dictionary){
            dictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            CreateCollectionMembers(GetMembers(session, typesInfo.ExtendedCollectionMemberInfoType), dictionary);
            CreateReferenceMembers(GetMembers(session, typesInfo.ExtendedReferenceMemberInfoType), dictionary);
            CreateCoreMembers(GetMembers(session, typesInfo.ExtendedCoreMemberInfoType), dictionary);
        }

        public IEnumerable<IExtendedMemberInfo> GetMembers(Session session,Type infoType) {
            IEnumerable<IExtendedMemberInfo> extendedMemberInfos = new XPCollection(session, infoType).Cast<IExtendedMemberInfo>();
            return extendedMemberInfos.Where(info => !memberExists(info));
        }

        private bool memberExists(IExtendedMemberInfo info){
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name) != null;
        }

        public void CreateCollectionMembers(IEnumerable<IExtendedMemberInfo> xpCollection, XPDictionary dictionary){
//            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            foreach (var info in collection) {
//                var type = ReflectionHelper.FindType(info.TypeAttributes.OfType<IPersistentAssociationAttribute>().Single().ElementTypeFullName);
//                var genericType = typeof(XPCollection<>).MakeGenericType(type);
//                var memberInfo = XafTypesInfo.Instance.FindTypeInfo(info.Owner).CreateMember(info.Name, genericType);
//                CreateAttributes(info, memberInfo);
                XPCustomMemberInfo member = dictionary.GetClassInfo(info.Owner).CreateMember(info.Name, typeof(XPCollection), true);
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateReferenceMembers(IEnumerable<IExtendedMemberInfo> xpCollection, XPDictionary dictionary){
//            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()){
                                XPCustomMemberInfo memberInfo = dictionary.GetClassInfo(info.Owner).CreateMember(info.Name, info.ReferenceType);
//                var memberInfo = XafTypesInfo.Instance.FindTypeInfo(info.Owner).CreateMember(info.Name,info.ReferenceType);
                CreateAttributes(info, memberInfo);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateCoreMembers(IEnumerable<IExtendedMemberInfo> collection, XPDictionary dictionary){
            foreach (var info in collection.Cast<IExtendedCoreTypeMemberInfo>()){
                XPCustomMemberInfo member = dictionary.GetClassInfo(info.Owner).CreateMember(info.Name, Type.GetType("System." + info.DataType, true));
                var memberType = Type.GetType("System." + info.DataType, true);
//                var member = XafTypesInfo.Instance.FindTypeInfo(info.Owner).CreateMember(info.Name, memberType);
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo memberInfo) {
            foreach (AttributeInfo attributeInfo in extendedMemberInfo.TypeAttributes.Select(typeAttribute => typeAttribute.Create())) {
                memberInfo.AddAttribute((Attribute)ReflectionHelper.CreateObject(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues));
            }
        }
    }
}