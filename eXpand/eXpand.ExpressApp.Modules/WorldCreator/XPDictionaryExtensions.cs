using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator {
    public static class XPDictionaryExtensions
    {
        public static void AddClasses(this XPDictionary xpDictionary, List<IPersistentClassInfo> persistentClassInfos) {
            var builder = PersistentClassTypeBuilder.BuildClass();
            foreach (IPersistentClassInfo persistentClassInfo in persistentClassInfos) {
                CreateClass(xpDictionary, persistentClassInfo,builder);
            }
            foreach (IPersistentClassInfo classInfo in persistentClassInfos) {
                CreateMembers( classInfo);
            }
        }

        public static XPClassInfo AddClass(this XPDictionary xpDictionary, IPersistentClassInfo info) {
            var builder = PersistentClassTypeBuilder.BuildClass();
            XPClassInfo xpClassInfo = CreateClass(xpDictionary, info,builder);
            CreateMembers(info);
            return xpClassInfo;
        }

        private static XPClassInfo CreateClass(XPDictionary xpDictionary, IPersistentClassInfo info, IClassAssemblyNameBuilder builder)
        {
            XPClassInfo result = xpDictionary.QueryClassInfo(info.AssemblyName, info.Name);
            if (result == null)
            {
                //                XPClassInfo baseClassInfo = info.BaseClass != null ? xpDictionary.AddClass(info.BaseClass) : xpDictionary.GetClassInfo(info.GetDefaultBaseClass());
                var type = builder.WithAssemblyName(info.AssemblyName).Define(info);
                result = new ReflectionClassInfo(type, xpDictionary);
                CreateAttributes(result, info);
            }
            return result;
        }

        

        private static void CreateAttributes(XPTypeInfo ti,IPersistentTypeInfo info)
        {
            foreach (IPersistentAttributeInfo a in info.TypeAttributes) {
                ti.AddAttribute(a.Create());
            }
        }


        private static void CreateMembers(IPersistentClassInfo info)
        {
            foreach (IPersistentMemberInfo mi in info.OwnMembers) {
                CreateMember(mi);
            }
        }

        private static void CreateMember(IPersistentMemberInfo info)
        {
            XPMemberInfo result = info.Owner.PersistentTypeClassInfo.FindMember(info.Name) ?? CreateMemberCore( info);
            CreateAttributes(result,info);
        }

        private static XPMemberInfo CreateMemberCore(IPersistentMemberInfo info) {
            if (info is IPersistentCollectionMemberInfo)
                return info.Owner.PersistentTypeClassInfo.CreateMember(info.Name, typeof(XPCollection), true);
            if (info is IPersistentCoreTypeMemberInfo)
                return info.Owner.PersistentTypeClassInfo.CreateMember(info.Name, Type.GetType("System." + ((IPersistentCoreTypeMemberInfo)info).DataType, true));
            if (info is IPersistentReferenceMemberInfo) {
                var xpClassInfo = info.Owner.PersistentTypeClassInfo.Dictionary.GetClassInfo(((IPersistentReferenceMemberInfo)info).ReferenceType);
                return info.Owner.PersistentTypeClassInfo.CreateMember(info.Name, xpClassInfo);
            }
            throw new NotImplementedException(info.GetType().FullName);
        }
    }

    
}