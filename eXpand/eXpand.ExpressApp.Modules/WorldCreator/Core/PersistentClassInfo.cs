using System;
using System.Linq;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class PersistentClassInfoExtensions {
        public static void CreateMembersFromInterfaces(this IPersistentClassInfo classInfo) {
            foreach (IInterfaceInfo interfaceInfo in classInfo.Interfaces) {
                foreach (PropertyInfo propertyInfo in interfaceInfo.Type.GetProperties()) {
                    PropertyInfo info1 = propertyInfo;
                    bool propertyNotExists =
                        classInfo.OwnMembers.Where(info => info.Name == info1.Name).FirstOrDefault() == null;
                    if (propertyNotExists) {
                        IPersistentMemberInfo persistentMemberInfo = GetPersistentMemberInfo(classInfo, propertyInfo,TypesInfo.Instance,interfaceInfo);
                        classInfo.OwnMembers.Add(persistentMemberInfo);
                    }
                }
            }
        }

        static IPersistentMemberInfo GetPersistentMemberInfo(IPersistentClassInfo classInfo, PropertyInfo propertyInfo,
                                                             TypesInfo typesInfo,IInterfaceInfo interfaceInfo) {
            Type memberInfoType = GetMemberInfoType(propertyInfo.PropertyType, typesInfo);
            var persistentMemberInfo =
                ((IPersistentMemberInfo) Activator.CreateInstance(memberInfoType, classInfo.Session));
            persistentMemberInfo.CodeTemplateInfo =
                (ICodeTemplateInfo) Activator.CreateInstance(TypesInfo.Instance.CodeTemplateInfoType, classInfo.Session);
            ICodeTemplate defaultTemplate = CodeTemplateBuilder.CreateDefaultTemplate(TemplateType.InterfaceReadWriteMember, persistentMemberInfo.Session,
                                                                                      typesInfo.CodeTemplateType,
                                                                                      classInfo.PersistentAssemblyInfo.CodeDomProvider);
            defaultTemplate.TemplateCode = defaultTemplate.TemplateCode.Replace("$INTERFACENAME$", interfaceInfo.Name);
            persistentMemberInfo.CodeTemplateInfo.CodeTemplate= defaultTemplate;
            
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