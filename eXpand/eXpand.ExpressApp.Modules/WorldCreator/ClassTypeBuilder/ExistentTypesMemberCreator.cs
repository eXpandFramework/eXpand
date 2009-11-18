using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public class ExistentTypesMemberCreator
    {
        public void CreateMembers(Session session, TypesInfo typesInfo)
        {
            
            createCollectionMembers(getMembers(session, typesInfo.ExtendedCollectionMemberInfoType));
            createReferenceMembers(getMembers(session, typesInfo.ExtendedReferenceMemberInfoType));
            createCoreMembers(getMembers(session, typesInfo.ExtendedCoreMemberInfoType));
        }

        IEnumerable<IExtendedMemberInfo> getMembers(Session session,Type infoType) {
            IEnumerable<IExtendedMemberInfo> extendedMemberInfos = new XPCollection(session, infoType).Cast<IExtendedMemberInfo>();
            return extendedMemberInfos.Where(info => !memberExists(info));
        }

        private bool memberExists(IExtendedMemberInfo info)
        {
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name) != null;
        }

        void createCollectionMembers(IEnumerable<IExtendedMemberInfo> xpCollection)
        {
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            var collection = xpCollection.Cast<IExtendedCollectionMemberInfo>();
            foreach (var info in collection){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, typeof(XPCollection), true);
                createAttributes(info, member);
            }
        }

        void createReferenceMembers(IEnumerable<IExtendedMemberInfo> xpCollection)
        {
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (var info in xpCollection.Cast<IExtendedReferenceMemberInfo>()){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, info.ReferenceType);
                createAttributes(info, member);
            }
        }

        void createCoreMembers(IEnumerable<IExtendedMemberInfo> collection)
        {
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (var info in collection.Cast<IExtendedCoreTypeMemberInfo>()){
                XPCustomMemberInfo member = xpDictionary.GetClassInfo(info.Owner).CreateMember(info.Name, Type.GetType("System." + info.DataType, true));
                createAttributes(info, member);
            }
        }
        private void createAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo customMemberInfo)
        {
            foreach (IPersistentAttributeInfo typeAttribute in extendedMemberInfo.TypeAttributes){
                AttributeInfo attributeInfo = typeAttribute.Create();
                customMemberInfo.AddAttribute((Attribute)Activator.CreateInstance(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues));
            }
        }

    }
}