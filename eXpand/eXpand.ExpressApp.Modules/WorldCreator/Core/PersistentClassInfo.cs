using System;
using System.Linq;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class PersistentClassInfoExtensions {
        public static void CreateMembersFromInterfaces(this IPersistentClassInfo classInfo, TypesInfo typesInfo) {
            foreach (IInterfaceInfo interfaceInfo in classInfo.Interfaces) {
                foreach (PropertyInfo propertyInfo in interfaceInfo.Type.GetProperties()) {
                    PropertyInfo info1 = propertyInfo;
                    bool propertyNotExists =
                        classInfo.OwnMembers.Where(info => info.Name == info1.Name).FirstOrDefault() == null;
                    if (propertyNotExists) {
                        IPersistentMemberInfo persistentMemberInfo = GetPersistentMemberInfo(classInfo, propertyInfo,typesInfo);
                        classInfo.OwnMembers.Add(persistentMemberInfo);
                    }
                }
            }
        }

        static IPersistentMemberInfo GetPersistentMemberInfo(IPersistentClassInfo classInfo, PropertyInfo propertyInfo,
                                                             TypesInfo typesInfo) {
            Type memberInfoType = GetMemberInfoType(propertyInfo.PropertyType, typesInfo);
            var persistentMemberInfo =
                ((IPersistentMemberInfo) Activator.CreateInstance(memberInfoType, classInfo.Session));
            var codeTemplate = (ICodeTemplate) Activator.CreateInstance(typesInfo.CodeTemplateType, classInfo.Session);
            codeTemplate.TemplateType=TemplateType.ReadWriteMember;
            codeTemplate.SetDefaults();
            persistentMemberInfo.CodeTemplate =codeTemplate;
            persistentMemberInfo.Name = propertyInfo.Name;
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                ((IPersistentCoreTypeMemberInfo) persistentMemberInfo).DataType =
                    (XPODataType) Enum.Parse(typeof (XPODataType), propertyInfo.PropertyType.Name);
            else if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                ((IPersistentReferenceMemberInfo) persistentMemberInfo).ReferenceTypeFullName = propertyInfo.PropertyType.FullName;
            return persistentMemberInfo;
        }

        static Type GetMemberInfoType(Type propertyType, TypesInfo typesInfo) {
            if (typeof (IXPSimpleObject).IsAssignableFrom(propertyType))
                return typesInfo.PersistentReferenceInfoType;
            var i = ((int) Enum.Parse(typeof (XPODataType), propertyType.Name));
            if (i > -1)
                return typesInfo.PersistentCoreTypeInfoType;
            throw new NotImplementedException(propertyType.ToString());
        }
    }
}