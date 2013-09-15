using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;
using Fasterflect;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class PersistentClassInfoExtensions {

        public static IPersistentMemberInfo CreateCollection(this IPersistentClassInfo classInfo, string assemblyName, string classInfoName) {
            var collectionMemberInfo = XPObjectSpace.FindObjectSpaceByObject(classInfo).CreateWCObject<IPersistentCollectionMemberInfo>();
            collectionMemberInfo.Owner = classInfo;
            collectionMemberInfo.Name = classInfoName + "s";
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
                if (memberInfo != null)
                    memberInfo.SetReferenceTypeFullName(propertyInfo.PropertyType.FullName);
            }
        }


        static Type GetMemberInfoType(Type propertyType) {
            if (typeof(IXPSimpleObject).IsAssignableFrom(propertyType))
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