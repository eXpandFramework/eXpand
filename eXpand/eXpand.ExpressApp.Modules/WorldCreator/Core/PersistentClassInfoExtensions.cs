using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class PersistentClassInfoExtensions {
        
        public static IPersistentMemberInfo CreateCollection(this IPersistentClassInfo classInfo, string assemblyName, string classInfoName) {
            var collectionMemberInfo =ObjectSpace.FindObjectSpace(classInfo).CreateWCObject<IPersistentCollectionMemberInfo>();
            collectionMemberInfo.Owner=classInfo;
            collectionMemberInfo.Name = classInfoName+"s";
            collectionMemberInfo.SetCollectionTypeFullName(assemblyName + "." + classInfoName);
            collectionMemberInfo.Init(WCTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>());
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
                ((IPersistentMemberInfo)ReflectionHelper.CreateObject(memberInfoType, classInfo.Session));
            classInfo.OwnMembers.Add(persistentMemberInfo);
            persistentMemberInfo.SetDefaultTemplate(TemplateType.InterfaceReadWriteMember);
            persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode =
                persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode.Replace("$INTERFACENAME$",interfaceInfo.Name);

            persistentMemberInfo.Name = propertyInfo.Name;
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                ((IPersistentCoreTypeMemberInfo) persistentMemberInfo).DataType =
                    (DBColumnType)Enum.Parse(typeof(DBColumnType), propertyInfo.PropertyType.Name);
            else if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                ((IPersistentReferenceMemberInfo)persistentMemberInfo).SetReferenceTypeFullName(propertyInfo.PropertyType.FullName);
            
        }


        static Type GetMemberInfoType(Type propertyType) {
            if (typeof (IXPSimpleObject).IsAssignableFrom(propertyType))
                return WCTypesInfo.Instance.FindBussinessObjectType<IPersistentReferenceMemberInfo>();
            var i = ((int)Enum.Parse(typeof(DBColumnType), propertyType.Name));
            if (i > -1)
                return WCTypesInfo.Instance.FindBussinessObjectType<IPersistentCoreTypeMemberInfo>();
            throw new NotImplementedException(propertyType.ToString());
        }
    }

    public interface ICollectionMemberHandler {
    }
}