using System;
using System.Linq;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class PersistentClassInfoExtensions {
        
        public static IPersistentMemberInfo CreateCollection(this IPersistentClassInfo classInfo, string assemblyName, string classInfoName) {
            var collectionMemberInfo =
                (IPersistentCollectionMemberInfo)
                Activator.CreateInstance(TypesInfo.Instance.PersistentCollectionInfoType, classInfo.Session);

            collectionMemberInfo.Owner=classInfo;
            collectionMemberInfo.Name = classInfoName+"s";
            collectionMemberInfo.CollectionTypeFullName =assemblyName+"."+ classInfoName;
            collectionMemberInfo.Init(TypesInfo.Instance.CodeTemplateType);
            collectionMemberInfo.CodeTemplateInfo.CloneProperties();
            classInfo.OwnMembers.Add(collectionMemberInfo);
            return collectionMemberInfo;
        }

        public static void CreateMembersFromInterfaces(this IPersistentClassInfo classInfo) {
            foreach (IInterfaceInfo interfaceInfo in classInfo.Interfaces) {
                foreach (PropertyInfo propertyInfo in interfaceInfo.Type.GetProperties()) {
                    PropertyInfo info1 = propertyInfo;
                    bool propertyNotExists =
                        classInfo.OwnMembers.Where(info => info.Name == info1.Name).FirstOrDefault() == null;
                    if (propertyNotExists) {
                        AddPersistentMemberInfo(classInfo, propertyInfo,interfaceInfo);
                    }
                }
            }
        }

        static void AddPersistentMemberInfo(IPersistentClassInfo classInfo, PropertyInfo propertyInfo, IInterfaceInfo interfaceInfo) {
            
            Type memberInfoType = GetMemberInfoType(propertyInfo.PropertyType);
            var persistentMemberInfo =
                ((IPersistentMemberInfo)Activator.CreateInstance(memberInfoType, classInfo.Session));
            classInfo.OwnMembers.Add(persistentMemberInfo);
            persistentMemberInfo.SetDefaultTemplate(TemplateType.InterfaceReadWriteMember);
            persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode =
                persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode.Replace("$INTERFACENAME$",interfaceInfo.Name);

            persistentMemberInfo.Name = propertyInfo.Name;
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                ((IPersistentCoreTypeMemberInfo) persistentMemberInfo).DataType =
                    (XPODataType) Enum.Parse(typeof (XPODataType), propertyInfo.PropertyType.Name);
            else if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                ((IPersistentReferenceMemberInfo) persistentMemberInfo).ReferenceTypeFullName = propertyInfo.PropertyType.FullName;
            
        }


        static Type GetMemberInfoType(Type propertyType) {
            if (typeof (IXPSimpleObject).IsAssignableFrom(propertyType))
                return TypesInfo.Instance.PersistentReferenceInfoType;
            var i = ((int) Enum.Parse(typeof (XPODataType), propertyType.Name));
            if (i > -1)
                return TypesInfo.Instance.PersistentCoreTypeInfoType;
            throw new NotImplementedException(propertyType.ToString());
        }
    }

    public interface ICollectionMemberHandler {
    }
}