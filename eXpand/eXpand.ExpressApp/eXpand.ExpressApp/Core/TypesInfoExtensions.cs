using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Core{
    public static class TypesInfoExtensions{
        public static Type FindBussinessObjectType<T>(this ITypesInfo typesInfo)  
        {
            if (!(typeof(T).IsInterface))
                throw new ArgumentException(typeof(T).FullName +" should be an interface");
            var implementors = typesInfo.FindTypeInfo(typeof(T)).Implementors.Where(info => info.IsPersistent);
            var objectType = implementors.FirstOrDefault();
            if (objectType== null)
                throw new ArgumentException("No bussincess object that implements "+typeof(T).FullName+" found");
            if (implementors.Count() > 1) {
                var typeInfos = implementors.Where(implementor => !(typeof (T).IsAssignableFrom(implementor.Base.Type)));
                foreach (ITypeInfo implementor in typeInfos) {
                    return implementor.Type;
                }

                throw new ArgumentNullException("More than 1 objects implement " + typeof(T).FullName);
            }
            return objectType.Type;
        }
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

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName,
                                                          string propertyName, XPDictionary dictionary) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary,propertyName);
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
                if (xpClassInfo.FindMember(propertyName)== null) {
                    member = xpClassInfo.CreateMember(propertyName, typeOfMember);
                    member.AddAttribute(new AssociationAttribute(associationName));
                    typesInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;
        }

        public static ITypeInfo FindTypeInfo<T>(this ITypesInfo typesInfo)
        {
            return typesInfo.FindTypeInfo(typeof(T));
        }
        public static void AddAttribute<T>(this ITypeInfo typeInfo, Expression<Func<T, object>> expression,Attribute attribute)
        {
            IMemberInfo controlTypeMemberInfo = typeInfo.FindMember(expression);
            controlTypeMemberInfo.AddAttribute(attribute);
        }
        public static IMemberInfo FindMember<T>(this ITypeInfo typesInfo, Expression<Func<T, object>> expression)
        {
            MemberInfo memberInfo = ReflectionExtensions.GetExpression(expression);
            return typesInfo.FindMember(memberInfo.Name);
        }
        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember,
                                        string associationName, string propertyName,XPDictionary dictionary) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, propertyName);
        }
            public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember,
                                        string associationName,XPDictionary dictionary){
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary,typeOfMember.Name);
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association)
        {
            var infos = new List<XPCustomMemberInfo>();
            XPCustomMemberInfo collection = CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary);
            if (collection!= null){
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