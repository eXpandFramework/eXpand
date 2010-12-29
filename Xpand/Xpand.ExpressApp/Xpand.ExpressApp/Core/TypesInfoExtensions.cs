using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Core {
    public static class TypesInfoExtensions {
        public static Type FindBussinessObjectType<T>(this ITypesInfo typesInfo) {
            if (!(typeof(T).IsInterface))
                throw new ArgumentException(typeof(T).FullName + " should be an interface");
            //            var implementors = typesInfo.FindTypeInfo(typeof(T)).Implementors.Where(info => info.IsPersistent);
            var implementors = typesInfo.FindTypeInfo(typeof(T)).Implementors;
            var objectType = implementors.FirstOrDefault();
            if (objectType == null)
                throw new ArgumentException("No bussincess object that implements " + typeof(T).FullName + " found");
            if (implementors.Count() > 1) {
                var typeInfos = implementors.Where(implementor => !(typeof(T).IsAssignableFrom(implementor.Base.Type)));
                foreach (ITypeInfo implementor in typeInfos) {
                    return implementor.Type;
                }

                throw new ArgumentNullException("More than 1 objects implement " + typeof(T).FullName);
            }
            return objectType.Type;
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, true);
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, bool refreshTypesInfo) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, typeOfMember.Name, refreshTypesInfo);
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, string propertyName) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, propertyName, true);
        }

        public static XPCustomMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, string propertyName, bool refreshTypesInfo) {
            XPCustomMemberInfo member = null;
            if (typeIsRegister(typesInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                if (xpClassInfo.FindMember(propertyName) == null) {
                    member = xpClassInfo.CreateMember(propertyName, typeOfMember);
                    member.AddAttribute(new AssociationAttribute(associationName));

                    if (refreshTypesInfo)
                        typesInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;
        }

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, true);
        }

        static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo,
                                                          string propertyName, bool isManyToMany) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary,
                                    propertyName, refreshTypesInfo,isManyToMany);
        }

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo,
                                                          string propertyName) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, propertyName, refreshTypesInfo,false);
        }

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, refreshTypesInfo, typeOfCollection.Name + "s");
        }

        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, collectionName, true);
        }

        static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName, bool refreshTypesInfo,
                                                          bool isManyToMany) {
            XPCustomMemberInfo member = null;
            if (typeIsRegister(typeInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                if (xpClassInfo.FindMember(collectionName) == null) {
                    member = xpClassInfo.CreateMember(collectionName, typeof(XPCollection), true);
                    member.AddAttribute(new AssociationAttribute(associationName, typeOfCollection) { UseAssociationNameAsIntermediateTableName = isManyToMany });

                    if (refreshTypesInfo)
                        typeInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;

        }
        public static XPCustomMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName, bool refreshTypesInfo) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary,collectionName,refreshTypesInfo,false);
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesInfo, Type typeToCreateOn, Type otherPartType, XPDictionary dictionary) {
            return CreateBothPartMembers(typesInfo, typeToCreateOn, otherPartType, dictionary, false);
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany) {
            return CreateBothPartMembers(typesinfo, typeToCreateOn, otherPartMember, xpDictionary, isManyToMany, Guid.NewGuid().ToString());
        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association,
                                                                     string createOnPropertyName, string otherPartPropertyName) {
            var infos = new List<XPCustomMemberInfo>();
            XPCustomMemberInfo member = isManyToMany
                                            ? CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association,
                                                               xpDictionary, false, createOnPropertyName,true)
                                            : CreateMember(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary, createOnPropertyName, false);

            if (member != null) {
                infos.Add(member);
                member = isManyToMany
                             ? CreateCollection(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary,
                                                false, otherPartPropertyName,true)
                             : CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary,
                                                false, otherPartPropertyName);

                if (member != null)
                    infos.Add(member);
            }

            typesinfo.RefreshInfo(typeToCreateOn);
            typesinfo.RefreshInfo(otherPartMember);
            return infos;

        }

        public static List<XPCustomMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association) {

            var infos = new List<XPCustomMemberInfo>();
            XPCustomMemberInfo member = isManyToMany
                                            ? CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary, false)
                                            : CreateMember(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary, false);

            if (member != null) {
                infos.Add(member);
                member = isManyToMany
                             ? CreateCollection(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary,
                                                false)
                             : CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary,
                                                false);

                if (member != null)
                    infos.Add(member);
            }

            typesinfo.RefreshInfo(typeToCreateOn);
            typesinfo.RefreshInfo(otherPartMember);

            return infos;
        }

        public static ITypeInfo FindTypeInfo<T>(this ITypesInfo typesInfo) {
            return typesInfo.FindTypeInfo(typeof(T));
        }

        public static void AddAttribute<T>(this ITypeInfo typeInfo, Expression<Func<T, object>> expression, Attribute attribute) {
            IMemberInfo controlTypeMemberInfo = typeInfo.FindMember(expression);
            controlTypeMemberInfo.AddAttribute(attribute);
        }

        public static IMemberInfo FindMember<T>(this ITypeInfo typesInfo, Expression<Func<T, object>> expression) {
            MemberInfo memberInfo = ReflectionExtensions.GetExpression(expression);
            return typesInfo.FindMember(memberInfo.Name);
        }

        private static bool typeIsRegister(ITypesInfo typeInfo, Type typeToCreateOn) {
            return typeInfo.PersistentTypes.Where(info => info.Type == typeToCreateOn).FirstOrDefault() != null;
        }
    }
}