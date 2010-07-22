using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class ExistentTypesMemberCreator
    {
        public void CreateMembers(Session session, TypesInfo typesInfo){
            CreateCollectionMembers(GetMembers(session, typesInfo.ExtendedCollectionMemberInfoType));
            CreateReferenceMembers(GetMembers(session, typesInfo.ExtendedReferenceMemberInfoType));
            CreateCoreMembers(GetMembers(session, typesInfo.ExtendedCoreMemberInfoType));
        }

        public IEnumerable<IExtendedMemberInfo> GetMembers(Session session,Type infoType) {
            IEnumerable<IExtendedMemberInfo> extendedMemberInfos = new XPCollection(session, infoType).Cast<IExtendedMemberInfo>();
            return extendedMemberInfos.Where(info => !memberExists(info));
        }

        private bool memberExists(IExtendedMemberInfo info){
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name) != null;
        }

        public void CreateCollectionMembers(IEnumerable<IExtendedMemberInfo> xpCollection){
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            foreach (var info in collection){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, typeof(XPCollection), true);
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateReferenceMembers(IEnumerable<IExtendedMemberInfo> xpCollection){
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, info.ReferenceType);
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateCoreMembers(IEnumerable<IExtendedMemberInfo> collection){
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (var info in collection.Cast<IExtendedCoreTypeMemberInfo>()){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, Type.GetType("System." + info.DataType, true));
                CreateAttributes(info, member);
                XafTypesInfo.Instance.RefreshInfo(info.Owner);
            }
        }

        public void CreateAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo customMemberInfo) {
            foreach (AttributeInfo attributeInfo in extendedMemberInfo.TypeAttributes.Select(typeAttribute => typeAttribute.Create())) {
                customMemberInfo.AddAttribute((Attribute)ReflectionHelper.CreateObject(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues));
            }
        }
    }
}