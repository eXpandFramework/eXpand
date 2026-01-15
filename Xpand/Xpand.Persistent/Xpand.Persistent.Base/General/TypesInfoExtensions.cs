using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Extensions.XAF.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General {

    public static class TypesInfoExtensions {
        public static IEnumerable<ITypeInfo> BaseInfos(this ITypeInfo typeInfo) {
            var baseInfo = typeInfo.Base;
            while (baseInfo != null) {
                yield return baseInfo;
                baseInfo = baseInfo.Base;
            }
        }

        public static IEnumerable<ITypeInfo> DomainSealedInfos(this ITypesInfo typesInfo,Type type){
            var typeInfo = typesInfo.FindTypeInfo(type);
            var infos = typeInfo.IsInterface ? typeInfo.Implementors : typeInfo.Descendants;
            var typeInfos = infos.Where(info => !info.IsAbstract).Reverse().ToArray();
            return typeInfos.Except(typeInfos.SelectMany(BaseInfos));
        }

        public static IEnumerable<ITypeInfo> DomainSealedInfos<T>(this ITypesInfo typesInfo){
            return typesInfo.DomainSealedInfos(typeof(T));
        }
        
        public static ITypeInfo GetITypeInfo(this object obj){
            return obj.GetType().GetITypeInfo();
        }

        public static ITypeInfo GetITypeInfo(this Type type){
            return XafTypesInfo.Instance.FindTypeInfo(type);
        }

        public static bool IsDomainComponent(this Type type){
            return type.Attribute<DomainComponentAttribute>() != null ||new ReflectionDictionary().QueryClassInfo(type) != null;
        }

        public static IModelClass ModelClass(this ITypeInfo typeInfo){
            return CaptionHelper.ApplicationModel.BOModel.GetClass(typeInfo.Type);
        }

        

        

        static readonly MemberSetter XpoTypeInfoSourceSetter = typeof(XpoTypesInfoHelper).DelegateForSetFieldValue("xpoTypeInfoSource");
        public static void AssignAsPersistentEntityStore(this XpoTypeInfoSource xpoTypeInfoSource){
            var entityStores = new List<IEntityStore>((IEntityStore[]) XafTypesInfo.Instance.GetFieldValue("entityStores"));
            entityStores.RemoveAll(store => store is XpoTypeInfoSource);
            XafTypesInfo.Instance.SetFieldValue("entityStores", entityStores.ToArray());
            XafTypesInfo.Instance.GetFieldValue("entityTypesCache").CallMethod("Clear");
            ((TypesInfo) XafTypesInfo.Instance).AddEntityStore(xpoTypeInfoSource);
            XpoTypeInfoSourceSetter(null, xpoTypeInfoSource);
        }

        public static void AssignAsInstance(this ITypesInfo typesInfo) {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            var type = typeof (XafTypesInfo);
            if (type.GetFieldValue("instance")!=typesInfo){
                type.SetFieldValue("instance", typesInfo);
                typeof(XpoTypesInfoHelper).SetFieldValue("xpoTypeInfoSource",((TypesInfo) typesInfo).FindEntityStore(typeof(XpoTypeInfoSource)));
            }
        }

        public static Type FindBusinessObjectType(this ITypesInfo typesInfo,Type type){
            if (!(type.IsInterface))
                return type;
            var implementors = typesInfo.FindTypeInfo(type).Implementors.ToArray();
            var objectType = implementors.FirstOrDefault();
            if (objectType == null)
                throw new ArgumentException("Add a business object that implements " +
                                                type.FullName + " at your AdditionalBusinessClasses (module.designer.cs)");
            if (implementors.Length > 1) {
                var typeInfos = implementors.Where(implementor => implementor.Base != null && !(type.IsAssignableFrom(implementor.Base.Type)));
                foreach (ITypeInfo implementor in typeInfos) {
                    return implementor.Type;
                }

                throw new ArgumentNullException("More than 1 objects implement " + type.FullName);
            }
            return objectType.Type;

        }

        public static Type FindBusinessObjectType<T>(this ITypesInfo typesInfo){
            return typesInfo.FindBusinessObjectType(typeof(T));
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName,  true);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName,  bool refreshTypesInfo) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName,  typeOfMember.Name, refreshTypesInfo);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName,  string propertyName) {
            return CreateMember(typesInfo, typeToCreateOn, typeOfMember, associationName,  propertyName, true);
        }

        public static XPMemberInfo CreateMember(this ITypesInfo typesInfo, Type typeToCreateOn, Type typeOfMember, string associationName,  string propertyName, bool refreshTypesInfo) {
            XPMemberInfo member = null;
            if (TypeIsRegister(typesInfo, typeToCreateOn)) {
                XPClassInfo xpClassInfo = typesInfo.FindTypeInfo(typeToCreateOn).QueryXPClassInfo();
                member = xpClassInfo.FindMember(propertyName);
                if (member == null) {
                    member = xpClassInfo.CreateMember(propertyName, typeOfMember,
                        new AssociationAttribute(associationName, typeOfMember));
                    if (refreshTypesInfo)
                        typesInfo.RefreshInfo(typeToCreateOn);
                }
            }
            return member;
        }

        public static XPMemberInfo CreateCollection(this ITypesInfo typeInfo, Type typeToCreateOn, Type typeOfCollection, string associationName) {
            return typeInfo.CreateCollection( typeToCreateOn, typeOfCollection, associationName,  true);
        }

        

        public static ITypeInfo FindTypeInfo<T>(this ITypesInfo typesInfo) {
            return typesInfo.FindTypeInfo(typeof(T));
        }

        public static void AddAttribute<T>(this ITypeInfo typeInfo, Expression<Func<T, object>> expression, Attribute attribute) {
            IMemberInfo controlTypeMemberInfo = typeInfo.FindMember(expression);
            controlTypeMemberInfo.AddAttribute(attribute);
        }

        public static IMemberInfo FindMember<T>(this ITypeInfo typesInfo, Expression<Func<T, object>> expression) {
            MemberInfo memberInfo = expression.GetMemberInfo();
            return typesInfo.FindMember(memberInfo.Name);
        }

        private static bool TypeIsRegister(ITypesInfo typeInfo, Type typeToCreateOn) {
            return XafTypesInfo.Instance.FindTypeInfo(typeToCreateOn).IsDomainComponent ||
                   typeInfo.PersistentTypes.FirstOrDefault(info => info.Type == typeToCreateOn) != null;
        }
    }
}