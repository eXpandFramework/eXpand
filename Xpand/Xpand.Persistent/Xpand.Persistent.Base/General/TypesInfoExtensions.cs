using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General {

    public static class TypesInfoExtensions {
        public static void AssignAsInstance(this ITypesInfo typesInfo) {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            typeof (XafTypesInfo).SetFieldValue("instance", typesInfo);
            var xpoTypeInfoSource = ((TypesInfo) typesInfo).EntityStores.OfType<XpoTypeInfoSource>().FirstOrDefault();
            typeof(XafTypesInfo).SetFieldValue("persistentEntityStore", xpoTypeInfoSource);
        }


        public static Type FindBussinessObjectType<T>(this ITypesInfo typesInfo) {
            if (!(typeof(T).IsInterface))
                throw new ArgumentException(typeof(T).FullName + " should be an interface");
            var implementors = typesInfo.FindTypeInfo(typeof(T)).Implementors;
            var objectType = implementors.FirstOrDefault();
            if (objectType == null)
                throw new ArgumentException("Add a business object that implements " +
                                                typeof(T).FullName + " at your AdditionalBusinessClasses (module.designer.cs)");
            if (implementors.Count() > 1) {
                var typeInfos = implementors.Where(implementor =>implementor.Base!=null&& !(typeof(T).IsAssignableFrom(implementor.Base.Type)));
                foreach (ITypeInfo implementor in typeInfos) {
                    return implementor.Type;
                }

                throw new ArgumentNullException("More than 1 objects implement " + typeof(T).FullName);
            }
            return objectType.Type;
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, true);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, bool refreshTypesInfo) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, typeOfMember.Name, refreshTypesInfo);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, string propertyName) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName, dictionary, propertyName, true);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName, XPDictionary dictionary, string propertyName, bool refreshTypesInfo) {
            XPMemberInfo member = null;
            if (TypeIsRegister(typesInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                member = xpClassInfo.FindMember(propertyName);
                if (member == null) {
                    member = xpClassInfo.CreateMember(propertyName, typeOfMember);
                    member.AddAttribute(new AssociationAttribute(associationName));

                    if (refreshTypesInfo)
                        typesInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;
        }

        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, true);
        }

        static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo,
                                                          string propertyName, bool isManyToMany) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary,
                                    propertyName, refreshTypesInfo, isManyToMany);
        }

        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo,
                                                          string propertyName) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, propertyName, refreshTypesInfo, false);
        }

        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, bool refreshTypesInfo) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, refreshTypesInfo, typeOfCollection.Name + "s");
        }

        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, collectionName, true);
        }

        static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName, bool refreshTypesInfo,
                                                          bool isManyToMany) {
            XPMemberInfo member = null;
            if (TypeIsRegister(typeInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = dictionary.GetClassInfo(typeToCreateOn);
                member = xpClassInfo.FindMember(collectionName);
                if (member == null) {
                    member = xpClassInfo.CreateMember(collectionName, typeof(XPCollection), true);
                    member.AddAttribute(new AssociationAttribute(associationName, typeOfCollection) { UseAssociationNameAsIntermediateTableName = isManyToMany });

                    if (refreshTypesInfo)
                        typeInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;

        }
        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName, XPDictionary dictionary, string collectionName, bool refreshTypesInfo) {
            return CreateCollection(typeInfo, typeToCreateOn, typeOfCollection, associationName, dictionary, collectionName, refreshTypesInfo, false);
        }

        public static List<XPMemberInfo> CreateBothPartMembers(this ITypesInfo typesInfo, Type typeToCreateOn, Type otherPartType, XPDictionary dictionary) {
            return CreateBothPartMembers(typesInfo, typeToCreateOn, otherPartType, dictionary, false);
        }

        public static List<XPMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany) {
            return CreateBothPartMembers(typesinfo, typeToCreateOn, otherPartMember, xpDictionary, isManyToMany, Guid.NewGuid().ToString());
        }

        public static List<XPMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association,
                                                                     string createOnPropertyName, string otherPartPropertyName) {
            var infos = new List<XPMemberInfo>();
            XPMemberInfo member = isManyToMany
                                            ? CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association,
                                                               xpDictionary, false, createOnPropertyName, true)
                                            : CreateMember(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary, createOnPropertyName, false);

            if (member != null) {
                infos.Add(member);
                member = isManyToMany
                             ? CreateCollection(typesinfo, otherPartMember, typeToCreateOn, association, xpDictionary,
                                                false, otherPartPropertyName, true)
                             : CreateCollection(typesinfo, typeToCreateOn, otherPartMember, association, xpDictionary,
                                                false, otherPartPropertyName);

                if (member != null)
                    infos.Add(member);
            }

            typesinfo.RefreshInfo(typeToCreateOn);
            typesinfo.RefreshInfo(otherPartMember);
            return infos;

        }

        public static List<XPMemberInfo> CreateBothPartMembers(this ITypesInfo typesinfo, Type typeToCreateOn, Type otherPartMember, XPDictionary xpDictionary, bool isManyToMany, string association) {

            var infos = new List<XPMemberInfo>();
            XPMemberInfo member = isManyToMany
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

        private static bool TypeIsRegister(ITypesInfo typeInfo, Type typeToCreateOn) {
            return XafTypesInfo.Instance.FindTypeInfo(typeToCreateOn).IsDomainComponent ||
                   typeInfo.PersistentTypes.FirstOrDefault(info => info.Type == typeToCreateOn) != null;
        }
    }
}