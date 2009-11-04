using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.Core{
    public static class TypesInfoExtensions{
        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName)
        {
            XPCustomMemberInfo member = null;
            if (typeIsRegister(typeInfo, typeToCreateOn))
            {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                if (xpClassInfo.FindMember(collectionName)== null){
                    member = xpClassInfo.CreateMember(collectionName, typeof(XPCollection), true);
                    member.AddAttribute(new AssociationAttribute(associationName, typeOfCollection));
                    typeInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;
        }

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary)
        {

            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, typeOfCollection.Name + "s");
        }

        private static bool typeIsRegister(ITypesInfo typeInfo, Type typeToCreateOn){
            return typeInfo.PersistentTypes.Where(info => info.Type == typeToCreateOn).FirstOrDefault() != null;
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesInfo, Type typeToCreateOn, Type otherPartType,
                                                 XPDictionary dictionary){
            return CreateBothPartMembers(typesInfo, typeToCreateOn, otherPartType, Guid.NewGuid().ToString(), dictionary);
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesInfo, Type otherPartType, Type typeOfMember,
                                                 string associationName, XPDictionary dictionary){
            var customMemberInfos = new List<XPCustomMemberInfo>();
            XPCustomMemberInfo members = CreateMember(typesInfo, otherPartType, typeOfMember, associationName,dictionary);
            if (members!= null)
            {
                customMemberInfos.Add(members);
                members = CreateCollection(typesInfo, typeOfMember, otherPartType, associationName, dictionary);
                if (members!= null)
                    customMemberInfos.Add(members);
            }
            return customMemberInfos;
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember,
                                        string associationName, XPDictionary dictionary, string propertyName) {
            XPCustomMemberInfo member = null;
            if (typeIsRegister(typesInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                member = xpClassInfo.CreateMember(propertyName, typeOfMember);
                member.AddAttribute(new AssociationAttribute(associationName));
                typesInfo.RefreshInfo(typeToCreateOn);
            }
            return member;
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember,
                                        string associationName,XPDictionary dictionary){
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary,typeOfMember.Name);
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association)
        {
            var infos = new List<XPCustomMemberInfo>();
            XPCustomMemberInfo collection = CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary);
            if (collection!= null)
            {
                infos.Add(collection);
                collection = CreateCollection(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary);
                if (collection!= null)
                    infos.Add(collection);
            }
            return infos;
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany)
        {
            Guid guid = Guid.NewGuid();
            return CreateBothPartMembers(typesinfo, typeToCreateOn, otherPartMember, xpDictionary,isManyToMany,guid.ToString());
        }
    }
}