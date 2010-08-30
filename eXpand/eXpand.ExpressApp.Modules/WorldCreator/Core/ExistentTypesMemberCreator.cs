using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class ExistentTypesMemberCreator
    {
        public void CreateMembers(Session session){
            CreateCollectionMembers(session);
            CreateReferenceMembers(session);
            CreateCoreMembers(session);
        }

        public IEnumerable<IExtendedMemberInfo> GetMembers(Session session,Type infoType) {
            IEnumerable<IExtendedMemberInfo> extendedMemberInfos = new XPCollection(session, infoType).Cast<IExtendedMemberInfo>();
            return extendedMemberInfos.Where(info => !memberExists(info));
        }

        private bool memberExists(IExtendedMemberInfo info){
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name) != null;
        }

        public void CreateCollectionMembers(Session session){
            IEnumerable<IExtendedMemberInfo> xpCollection = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCollectionMemberInfo>());
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            foreach (var info in collection) {
                XPCustomMemberInfo member = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, typeof(XPCollection), true);
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateReferenceMembers(Session session){
            var xpCollection = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedReferenceMemberInfo>());
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()){
                XPCustomMemberInfo memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, info.ReferenceType);
                CreateAttributes(info, memberInfo);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateCoreMembers(Session session){
            var collection = GetMembers(session, WCTypesInfo.Instance.FindBussinessObjectType<IExtendedCoreTypeMemberInfo>());
            foreach (var info in collection.Cast<IExtendedCoreTypeMemberInfo>()){
                XPCustomMemberInfo member = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, Type.GetType("System." + info.DataType, true));
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