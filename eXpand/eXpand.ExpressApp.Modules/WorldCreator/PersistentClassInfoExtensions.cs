using System;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using System.Linq;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator
{
    public static class MemberInfoCreator
    {
        public static void CreateFromInterfaces(this IPersistentClassInfo classInfo,ITypesInfo typesInfo)
        {
            foreach (IInterfaceInfo interfaceInfo in classInfo.Interfaces) {
                foreach (var propertyInfo in interfaceInfo.Type.GetProperties()) {
                    PropertyInfo info1 = propertyInfo;
                    bool  propertyNotExists = classInfo.OwnMembers.Where(info => info.Name==info1.Name).FirstOrDefault()== null;
                    if (propertyNotExists) {
                        Type memberInfoType = GetMemberInfoType(propertyInfo.PropertyType,typesInfo);
                        var persistentMemberInfo = ((IPersistentMemberInfo) Activator.CreateInstance(memberInfoType,classInfo.Session));
                        persistentMemberInfo.Name = propertyInfo.Name;
                        classInfo.OwnMembers.Add(persistentMemberInfo);
                    }
                }
            }
        }

        static Type GetMemberInfoType(Type propertyType, ITypesInfo typesInfo) {
            if (typeof(IXPSimpleObject).IsAssignableFrom(propertyType))
                return typesInfo.ExtendedReferenceMemberInfoType;
            if (Enum.Parse(typeof(XPODataType),propertyType.Name)==(object) 0)
                return typesInfo.ExtendedCoreMemberInfoType;
            throw new NotImplementedException(propertyType.ToString());
        }
    }
}
