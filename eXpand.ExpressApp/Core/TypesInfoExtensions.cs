using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.Core{
    public static class TypesInfoExtensions{
        public static bool CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary,string typeToCreateOnCollectionName)
        {
            if (typeIsRegister(typeInfo, typeToCreateOn))
            {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                XPCustomMemberInfo member = xpClassInfo.CreateMember(typeToCreateOnCollectionName, typeof(XPCollection), true);
                member.AddAttribute(new AssociationAttribute(associationName, typeOfCollection));
                typeInfo.RefreshInfo(typeToCreateOn);
                return true;
            }
            return false;
        }

        public static bool CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection,string associationName, XPDictionary dictionary){

            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, typeOfCollection.Name + "s");
        }

        private static bool typeIsRegister(ITypesInfo typeInfo, Type typeToCreateOn){
            return typeInfo.PersistentTypes.Where(info => info.Type == typeToCreateOn).FirstOrDefault() != null;
        }

        public static bool CreateBothPartMembers(this ITypesInfo typesInfo, Type typeToCreateOn, Type otherPartType,
                                                 XPDictionary dictionary){
            return CreateBothPartMembers(typesInfo, typeToCreateOn, otherPartType, Guid.NewGuid().ToString(), dictionary);
        }

        public static bool CreateBothPartMembers(this ITypesInfo typesInfo, Type otherPartType, Type typeOfMember,
                                                 string associationName, XPDictionary dictionary){
            bool members = CreateMember(typesInfo, otherPartType, typeOfMember, associationName,dictionary);
            if (members)
                members = CreateCollection(typesInfo, typeOfMember, otherPartType, associationName, dictionary);
            return members;
        }

        public static bool CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember,
                                        string associationName,XPDictionary dictionary){
            if (typeIsRegister(typesInfo, typeToCreateOn)){
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                XPCustomMemberInfo member = xpClassInfo.CreateMember(typeOfMember.Name, typeOfMember);
                member.AddAttribute(new AssociationAttribute(associationName));
                typesInfo.RefreshInfo(typeToCreateOn);
                return true;
            }
            return false;
        }

        public static bool CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association){

            bool collection = CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary);
            if (collection)
                collection = CreateCollection(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary);
            return collection;
        }

        public static bool CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany){
            Guid guid = Guid.NewGuid();
            return CreateBothPartMembers(typesinfo, typeToCreateOn, otherPartMember, xpDictionary,isManyToMany,guid.ToString());
        }
    }
}