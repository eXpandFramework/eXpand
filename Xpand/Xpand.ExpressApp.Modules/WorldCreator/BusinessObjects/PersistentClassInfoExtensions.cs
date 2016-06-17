using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Fasterflect;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects {
    public static class PersistentClassInfoExtensions {
        public static IPersistentReferenceMemberInfo CreateReferenceMember(this IPersistentClassInfo classInfo,
            string name, string referenceTypeFullName,bool assocation=false){
            var referenceMember = classInfo.CreateMember<IPersistentReferenceMemberInfo>(name);
            referenceMember.SetReferenceTypeFullName(referenceTypeFullName);
            referenceMember.Init(XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>());
            referenceMember.CodeTemplateInfo.CloneProperties();
            if (assocation){
                var associationName = referenceMember.ReferenceClassInfo.Name+"-"+classInfo.Name;
                referenceMember.ReferenceClassInfo.CreateCollection(classInfo).CreateAssociation(associationName);
                referenceMember.CreateAssociation(associationName);
            }
            return referenceMember;
        }

        public static IPersistentReferenceMemberInfo CreateReferenceMember(this IPersistentClassInfo classInfo, string name, Type referenceType, bool assocation=false){
            return classInfo.CreateReferenceMember(name, referenceType.FullName,assocation);
        }

        public static IPersistentReferenceMemberInfo CreateReferenceMember(this IPersistentClassInfo classInfo, IPersistentClassInfo referenceClassInfo, bool association=false){
            Guard.ArgumentNotNull(referenceClassInfo.Name, "referenceClassInfo.Name");
            return classInfo.CreateReferenceMember(referenceClassInfo.Name, referenceClassInfo,association);
        }

        public static IPersistentReferenceMemberInfo CreateReferenceMember(this IPersistentClassInfo classInfo, string name, IPersistentClassInfo referenceClassInfo, bool association=false){
            return classInfo.CreateReferenceMember(name,classInfo.PersistentAssemblyInfo.Name + "." + referenceClassInfo.Name,association);
        }

        private static T CreateMember<T>(this IPersistentClassInfo classInfo, string name) where T : IPersistentMemberInfo{
            var memberInfo = (T) Activator.CreateInstance(XafTypesInfo.Instance.FindBussinessObjectType<T>(), classInfo.Session);
            memberInfo.Name = name;
            classInfo.OwnMembers.Add(memberInfo);
            memberInfo.SetDefaultTemplate(TemplateType.XPReadWritePropertyMember);
            return memberInfo;
        }

        public static IPersistentCoreTypeMemberInfo CreateSimpleMember(this IPersistentClassInfo classInfo, DBColumnType dataType,string name){
            var memberInfo = classInfo.CreateMember<IPersistentCoreTypeMemberInfo>(name);
            memberInfo.DataType=dataType;
            return memberInfo;
        }

        public static IPersistentCollectionMemberInfo CreateCollection(this IPersistentClassInfo classInfo,string name,
            IPersistentClassInfo persistentClassInfo,bool association=true){
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType(typeof(IPersistentCollectionMemberInfo));
            var collectionMemberInfo = (IPersistentCollectionMemberInfo)Activator.CreateInstance(objectType, classInfo.Session);
            collectionMemberInfo.Owner = classInfo;
            collectionMemberInfo.Name = name;
            collectionMemberInfo.SetCollectionTypeFullName(persistentClassInfo.PersistentAssemblyInfo.Name + "." + persistentClassInfo.Name);
            collectionMemberInfo.Init(XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>());
            collectionMemberInfo.CodeTemplateInfo.CloneProperties();
            classInfo.OwnMembers.Add(collectionMemberInfo);
            if (association)
                collectionMemberInfo.CreateAssociation(collectionMemberInfo.CollectionTypeFullName);
            return collectionMemberInfo;

        }

        public static IPersistentAssociationAttribute CreateAssociation(this IPersistentAssociatedMemberInfo associatedMemberInfo,string associationName,  Type elementType=null){
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<IPersistentAssociationAttribute>();
            var associationAttribute =(IPersistentAssociationAttribute)Activator.CreateInstance(objectType,associatedMemberInfo.Session);
            associationAttribute.Owner = associatedMemberInfo;
            if (elementType != null) associationAttribute.ElementTypeFullName = elementType.FullName;
            associationAttribute.AssociationName = associationName;
            return associationAttribute;
        }

        public static IPersistentCollectionMemberInfo CreateCollection(this IPersistentClassInfo classInfo,  IPersistentClassInfo persistentClassInfo,bool association=false){
            return classInfo.CreateCollection(persistentClassInfo.Name + "s", persistentClassInfo,association);
        }

        public static void CreateMembersFromInterfaces(this IPersistentClassInfo classInfo) {
            foreach (IInterfaceInfo interfaceInfo in classInfo.Interfaces) {
                foreach (PropertyInfo propertyInfo in interfaceInfo.Type.GetProperties()) {
                    PropertyInfo info1 = propertyInfo;
                    bool propertyNotExists =classInfo.OwnMembers.FirstOrDefault(info => info.Name == info1.Name) == null;
                    if (propertyNotExists) {
                        AddPersistentMemberInfo(classInfo, propertyInfo, interfaceInfo);
                    }
                }
            }
        }

        static void AddPersistentMemberInfo(IPersistentClassInfo classInfo, PropertyInfo propertyInfo, IInterfaceInfo interfaceInfo) {

            Type memberInfoType = GetMemberInfoType(propertyInfo.PropertyType);
            var persistentMemberInfo =
                ((IPersistentMemberInfo)memberInfoType.CreateInstance(classInfo.Session));
            classInfo.OwnMembers.Add(persistentMemberInfo);
            persistentMemberInfo.SetDefaultTemplate(TemplateType.InterfaceReadWriteMember);
            persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode =
                persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode.Replace("$INTERFACENAME$", interfaceInfo.Name);

            persistentMemberInfo.Name = propertyInfo.Name;
            var info = persistentMemberInfo as IPersistentCoreTypeMemberInfo;
            if (info != null)
                info.DataType =
                    (DBColumnType)Enum.Parse(typeof(DBColumnType), propertyInfo.PropertyType.Name);
            else {
                var memberInfo = persistentMemberInfo as IPersistentReferenceMemberInfo;
                memberInfo?.SetReferenceTypeFullName(propertyInfo.PropertyType.FullName);
            }
        }


        static Type GetMemberInfoType(Type propertyType) {
            if (typeof(IXPSimpleObject).IsAssignableFrom(propertyType))
                return XafTypesInfo.Instance.FindBussinessObjectType<IPersistentReferenceMemberInfo>();
            var i = ((int)Enum.Parse(typeof(DBColumnType), propertyType.Name));
            if (i > -1)
                return XafTypesInfo.Instance.FindBussinessObjectType<IPersistentCoreTypeMemberInfo>();
            throw new NotImplementedException(propertyType.ToString());
        }
    }

    public interface ICollectionMemberHandler {
    }
}