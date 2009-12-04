using System;
using System.Collections.Generic;
using System.Linq;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator {
    internal static class CodeEngine {
        public static string GenerateCode(IPersistentClassInfo persistentClassInfo) {
            string allMemberGeneratedCode = GetMembersCode(persistentClassInfo.OwnMembers);
            string code = getCode(persistentClassInfo);
            code = code.Replace("$CLASSNAME$", persistentClassInfo.Name);
            code = code.Replace("$BASECLASSNAME$", getBaseType(persistentClassInfo).FullName);
            code = code.TrimEnd('}') + allMemberGeneratedCode + "}";
            return code;
        }

        static string getCode(IPersistentTypeInfo persistentClassInfo) {
            return string.IsNullOrEmpty((persistentClassInfo.GeneratedCode + "").Trim()) ? persistentClassInfo.CodeTemplate.TemplateCode : persistentClassInfo.GeneratedCode;
        }

        static Type getBaseType(IPersistentClassInfo persistentClassInfo) {
            return persistentClassInfo.BaseType??persistentClassInfo.GetDefaultBaseClass();
        }

        static string GetMembersCode(IEnumerable<IPersistentMemberInfo> persistentMemberInfos) {
            string allcode = null;
            foreach (var persistentMemberInfo in persistentMemberInfos) {
                string code = getCode(persistentMemberInfo);
                code = code.Replace("$PROPERTYNAME$", persistentMemberInfo.Name);
                code = code.Replace("$PROPERTYTYPE$", GetPropertyType(persistentMemberInfo));
                allcode += code + Environment.NewLine;
            }
            return allcode;
        }

        static string GetPropertyType(IPersistentMemberInfo persistentMemberInfo) {
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                return Type.GetType("System."+((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType).FullName;
            if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                return ((IPersistentReferenceMemberInfo) persistentMemberInfo).ReferenceType.FullName;
            throw new NotImplementedException(persistentMemberInfo.GetType().FullName);
        }

        public static string GenerateCode(IPersistentAttributeInfo persistentAttributeInfo) {
            AttributeInfo attributeInfo = persistentAttributeInfo.Create();
            var attribute = (Attribute)Activator.CreateInstance(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues);
            string args = attributeInfo.InitializedArgumentValues.Aggregate<object, string>(null, (current, argumentValue) => current + (argumentValue + ",")).TrimEnd(',');
            return string.Format("[{0}({1})]", attribute.GetType().FullName, args);
        }
    }
}