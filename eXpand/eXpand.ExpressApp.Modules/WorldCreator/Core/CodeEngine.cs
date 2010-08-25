using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class CodeEngine {
        public static string GenerateCode(ICodeTemplateInfo codeTemplateInfo) {
            string code = codeTemplateInfo.TemplateInfo.TemplateCode+"";
            code = code.Replace("$ASSEMBLYNAME$", codeTemplateInfo.PersistentAssemblyInfo.Name);
            code = code.Replace("$CLASSNAME$", codeTemplateInfo.TemplateInfo.Name);
            return code;
        }

        public static string GenerateCode(IPersistentMemberInfo persistentMemberInfo) {
            if (persistentMemberInfo.CodeTemplateInfo.TemplateInfo != null) {
                string code = persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
                if (code != null) {
                    code = code.Replace("$TYPEATTRIBUTES$", GetAttributesCode(persistentMemberInfo));
                    code = code.Replace("$PROPERTYNAME$", GetMemberName(persistentMemberInfo));
                    code = code.Replace("$PROPERTYTYPE$", GetPropertyTypeCode(persistentMemberInfo));
                    code = code.Replace("$INJECTCODE$", GetInjectCode(persistentMemberInfo));
                }
                return code;
            }
            return null;
        }

        static string GetMemberName(IPersistentMemberInfo persistentMemberInfo) {
            string memberName = CleanName(persistentMemberInfo.Name);
            if (persistentMemberInfo.Owner.Name == persistentMemberInfo.Name)
                memberName += "Member";
            return memberName;
        }

        public static string GenerateCode(IPersistentClassInfo persistentClassInfo) {
            if (persistentClassInfo.CodeTemplateInfo != null &&
                persistentClassInfo.CodeTemplateInfo.TemplateInfo != null) {
                string code = persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
                string attributesCode = GetAttributesCode(persistentClassInfo);
                if (code != null) {
                    code = code.Replace("$ASSEMBLYNAME$", persistentClassInfo.PersistentAssemblyInfo.Name);
                    code = code.Replace("$TYPEATTRIBUTES$", attributesCode);
                    code = code.Replace("$CLASSNAME$", CleanName(persistentClassInfo.Name));
                    code = code.Replace("$BASECLASSNAME$", persistentClassInfo.BaseTypeFullName + GetInterfacesCode(persistentClassInfo));
                    code = code.Replace("$INJECTCODE$", GetInjectCode(persistentClassInfo));
                    code = GetAllMembersCode(persistentClassInfo, code);
                    return code;
                }
            }
            return null;
        }

        static string CleanName(string name) {
            var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            string ret = regex.Replace(name+"", "");
            if (!(string.IsNullOrEmpty(ret)) && !char.IsLetter(ret, 0) && !System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#").IsValidIdentifier(ret))
                ret = string.Concat("_", ret);
            return ret;
        }

        static string GetInjectCode(IPersistentTypeInfo persistentTypeInfo) {
            return persistentTypeInfo.TemplateInfos.Aggregate<ITemplateInfo, string>(null, (current, codeTemplate) => current + (codeTemplate.TemplateCode + Environment.NewLine));
        }

        static string GetAllMembersCode(IPersistentClassInfo persistentClassInfo, string code) {
            string allMemberGeneratedCode = GetMembersCode(persistentClassInfo.OwnMembers);
            int anchorsFound=0;
            for (int i = code.Length-1; i > -1; i--) {
                if (code[i] == '}' && anchorsFound < 2)
                    anchorsFound++;
                if (anchorsFound==2) {
                    code = code.Substring(0, i) + allMemberGeneratedCode + code.Substring(i);
                    break;
                }
            }
            return code;
        }

        static string GetInterfacesCode(IPersistentClassInfo persistentClassInfo) {
            return persistentClassInfo.Interfaces.Aggregate<IInterfaceInfo, string>(null, (current, interfaceInfo) => current + ("," + interfaceInfo.Type.FullName));
        }


        static string GetAttributesCode(IPersistentTypeInfo persistentClassInfo) {
            return (persistentClassInfo.TypeAttributes.Aggregate<IPersistentAttributeInfo, string>(null, (current, persistentAttributeInfo) => current + GenerateCode(persistentAttributeInfo) + Environment.NewLine)+"").TrimEnd(Environment.NewLine.ToCharArray());
        }



        static string GetMembersCode(IEnumerable<IPersistentMemberInfo> persistentMemberInfos){
            Func<IPersistentMemberInfo, string> codeSelector = persistentMemberInfo => GenerateCode(persistentMemberInfo);
            Func<string, string, string> aggrecator = (current, code) => current + (code + Environment.NewLine);
            return persistentMemberInfos.Select(codeSelector).Aggregate(null, aggrecator);
        }

        static string GetPropertyTypeCode(IPersistentMemberInfo persistentMemberInfo){
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                return GetCorePropertyTypeCode(persistentMemberInfo);
            if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                return CleanFullName(((IPersistentReferenceMemberInfo)persistentMemberInfo).ReferenceTypeFullName);
            if (persistentMemberInfo is IPersistentCollectionMemberInfo)
                return CleanFullName(((IPersistentCollectionMemberInfo)persistentMemberInfo).CollectionTypeFullName);
            throw new NotImplementedException(persistentMemberInfo.GetType().FullName);
        }

        static string CleanFullName(string fullName) {
            var list = fullName.Split('.').ToList();
            var name = list.Last();
            list.Remove(name);
            name = CleanName(name);
            list.Add(name);
            return list.Aggregate<string, string>(null, (current, l) => current + (l + ".")).TrimEnd('.');
        }

        static string GetCorePropertyTypeCode(IPersistentMemberInfo persistentMemberInfo) {
            DBColumnType dbColumnType = ((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType;
            switch (dbColumnType)
            {
                case DBColumnType.Boolean:
                    return "bool";
                case DBColumnType.Byte:
                    return "byte";
                case DBColumnType.SByte:
                    return "sbyte";
                case DBColumnType.Char:
                    return "char";
                case DBColumnType.Decimal:
                    return "decimal";
                case DBColumnType.Single:
                    return "float";
                case DBColumnType.Double:
                    return "double";
                case DBColumnType.Int32:
                    return "int";
                case DBColumnType.UInt32:
                    return "uint";
                case DBColumnType.Int16:
                    return "short";
                case DBColumnType.UInt16:
                    return "ushort";
                case DBColumnType.Int64:
                    return "long";
                case DBColumnType.UInt64:
                    return "ulong";
                case DBColumnType.String:
                    return "string";
                case DBColumnType.DateTime:
                    return "DateTime";
                case DBColumnType.Guid:
                    return "Guid";
                case DBColumnType.TimeSpan:
                    return "TimeSpan";
                case DBColumnType.ByteArray:
                    return "byte[]";
                default:
                    return dbColumnType.ToString();
            }
        }

        public static string GenerateCode(IPersistentAttributeCreator persistentAttributeCreator){
            AttributeInfo attributeInfo = persistentAttributeCreator.Create();
            var attribute = (Attribute)ReflectionHelper.CreateObject(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues);
            Func<object, object> argSelector = getArgumentCode;
            string args = attributeInfo.InitializedArgumentValues.Length>0
                              ? attributeInfo.InitializedArgumentValues.Select(argSelector).Aggregate
                              <object, string>(null, (current, o) => current + (o + ",")).TrimEnd(',')
                              : null;
            string assemblyDecleration = null;
            if (persistentAttributeCreator is IPersistentAssemblyAttributeInfo)
                assemblyDecleration = "assembly: ";
            return string.Format("[{0}{1}({2})]",assemblyDecleration, attribute.GetType().FullName, args);
        }

        static object getArgumentCode(object argumentValue) {
            if (argumentValue is string)
                return @"@""" + argumentValue + @"""";
            if (argumentValue is Type)
                return "typeof("+ ((Type) argumentValue).FullName+")";
            if (argumentValue is Enum)
                return argumentValue.GetType().FullName+"."+argumentValue;
            if (argumentValue is bool)
                return argumentValue.ToString().ToLower();
            return argumentValue;
        }

        static string GetAssemblyAttributesCode(IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            string code = "";
            foreach (var assemblyAttributeInfo in persistentAssemblyInfo.Attributes) {
                code += GenerateCode(assemblyAttributeInfo) + Environment.NewLine;
                ((IXPInvalidateableObject) assemblyAttributeInfo).Invalidate();
            }
            return code.TrimEnd(Environment.NewLine.ToCharArray());
            return (persistentAssemblyInfo.Attributes.Aggregate<IPersistentAssemblyAttributeInfo, string>(null, (current, persistentAssemblyAttributeInfo) => current + GenerateCode(persistentAssemblyAttributeInfo) + Environment.NewLine) + "").TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string GenerateCode(IPersistentAssemblyInfo persistentAssemblyInfo) {

            var usingsDictionary=new Dictionary<string, string>();
            string generateAssemblyCode = GetAssemblyAttributesCode(persistentAssemblyInfo) + Environment.NewLine +
                                  GetModuleCode(persistentAssemblyInfo.Name) + Environment.NewLine;
            var generatedClassCode = new StringBuilder();
            var generatedUsings = new StringBuilder();
            foreach (var persistentClassInfo in persistentAssemblyInfo.PersistentClassInfos) {
                string code = GenerateCode(persistentClassInfo);
                
                var regex = new Regex(persistentAssemblyInfo.CodeDomProvider == CodeDomProvider.CSharp ? "(using [^;]*;\r\n)*" : "(Imports [^\r\n]*\r\n)*");
                string usingsString = string.Concat("", regex.Matches(code).Cast<Match>().Where(match1 => !string.IsNullOrEmpty(match1.Value)).Select(match => match.Value).FirstOrDefault());
                code=regex.Replace(code, "");
                if (!(usingsDictionary.ContainsKey(usingsString))) {
                    usingsDictionary.Add(usingsString, null);
                    generatedUsings.Append(usingsString);
                }
                generatedClassCode.Append(code);
                ((IXPInvalidateableObject) persistentClassInfo).Invalidate();
            }
            return generatedUsings + generateAssemblyCode + generatedClassCode;
//            string generateCode =GetAssemblyAttributesCode(persistentAssemblyInfo)+Environment.NewLine+GetModuleCode(persistentAssemblyInfo.Name)+Environment.NewLine+ persistentAssemblyInfo.PersistentClassInfos.
//                Aggregate<IPersistentClassInfo, string>(null, (current, persistentClassInfo) => current + (GenerateCode(persistentClassInfo) + Environment.NewLine));
            //return GroupUsings(generatedCode.ToString(),persistentAssemblyInfo.CodeDomProvider);
        }
        internal static string GetModuleCode(string assemblyName)
        {
            return "namespace " + assemblyName + "{public class Dynamic" + (assemblyName + "").Replace(".", "") + "Module:DevExpress.ExpressApp.ModuleBase{}}";
        }

        static string GroupUsings(string generateCode,CodeDomProvider codeDomProvider) {
            var regex = new Regex(codeDomProvider == CodeDomProvider.CSharp ? "(using [^;]*;\r\n)*" : "(Imports [^\r\n]*\r\n)*");
            if (generateCode != null) {
                string s =
                    regex.Matches(generateCode).Cast<Match>().Where(match1 => !string.IsNullOrEmpty(match1.Value)).Select(match => match.Value).Distinct().Aggregate<string, string>(null, (current, match) => current + match);
                return s +Environment.NewLine+ regex.Replace(generateCode, "");
            }
            return null;
        }

        static string GetCode(IPersistentReferenceMemberInfo key)
        {
            var referenceMemberInfos = key.ReferenceClassInfo.OwnMembers.OfType<IPersistentReferenceMemberInfo>();
            string ret = null;
            foreach (var referenceMemberInfo in referenceMemberInfos)
            {
                string refPropertyName = CleanName(key.Name) + "." + CleanName(referenceMemberInfo.Name);
                var persistentMemberInfo = referenceMemberInfo.ReferenceClassInfo.OwnMembers.Where(info => info.TypeAttributes.OfType<IPersistentKeyAttribute>().Count() > 0).SingleOrDefault();
                if (persistentMemberInfo != null)
                {
                    var refKeyName = CleanName(persistentMemberInfo.Name);
                    ret += @"if(" + refPropertyName + ".Session != Session){" +
                          refPropertyName + "=Session.GetObjectByKey<" + CleanName(referenceMemberInfo.ReferenceClassInfo.Name) + ">(" + refPropertyName + "." + refKeyName + ");"
                          + "}";
                }
            }
            return ret;
        }

        public static void SupportCompositeKeyPersistentObjects(IPersistentAssemblyInfo persistentAssemblyInfo, Func<ITemplateInfo, bool> templateInfoPredicate) {
            var keys = persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers).OfType<IPersistentReferenceMemberInfo>().Where(memberInfo => memberInfo.ReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Struct);
            foreach (var key in keys)
            {
                
                var templateInfo = key.Owner.TemplateInfos.Where(templateInfoPredicate).Single();
                templateInfo.TemplateCode = @"protected override void OnLoaded() {
                                                base.OnLoaded();
                                                " + GetCode(key) + @"
                                            }";

            }

        }
    }
}