using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class CodeEngine {
        public const string SupportPersistentObjectsAsAPartOfACompositeKey = "SupportPersistentObjectsAsAPartOfACompositeKey";
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

        public static string CleanName(string name) {
            var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            string ret = regex.Replace(name + "", "");
            if (!(string.IsNullOrEmpty(ret)) && !char.IsLetter(ret, 0) && !System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#").IsValidIdentifier(ret))
                ret = string.Concat("_", ret);
            return ret;
        }

        static string GetInjectCode(IPersistentTypeInfo persistentTypeInfo) {
            return persistentTypeInfo.TemplateInfos.Aggregate<ITemplateInfo, string>(null, (current, codeTemplate) => current + (codeTemplate.TemplateCode + Environment.NewLine));
        }

        static string GetAllMembersCode(IPersistentClassInfo persistentClassInfo, string code) {
            string allMemberGeneratedCode = GetMembersCode(persistentClassInfo.OwnMembers);
            int anchorsFound = 0;
            for (int i = code.Length - 1; i > -1; i--) {
                if (code[i] == '}' && anchorsFound < 2)
                    anchorsFound++;
                if (anchorsFound == 2) {
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
            return (persistentClassInfo.TypeAttributes.Aggregate<IPersistentAttributeInfo, string>(null, (current, persistentAttributeInfo) => current + GenerateCode(persistentAttributeInfo) + Environment.NewLine) + "").TrimEnd(Environment.NewLine.ToCharArray());
        }



        static string GetMembersCode(IEnumerable<IPersistentMemberInfo> persistentMemberInfos) {
            Func<IPersistentMemberInfo, string> codeSelector = GenerateCode;
            Func<string, string, string> aggrecator = (current, code) => current + (code + Environment.NewLine);
            return persistentMemberInfos.Select(codeSelector).Aggregate(null, aggrecator);
        }

        static string GetPropertyTypeCode(IPersistentMemberInfo persistentMemberInfo) {
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                return GetCorePropertyTypeCode(persistentMemberInfo);
            if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                return CleanFullName(((IPersistentReferenceMemberInfo)persistentMemberInfo).ReferenceTypeFullName);
            if (persistentMemberInfo is IPersistentCollectionMemberInfo)
                return CleanFullName(((IPersistentCollectionMemberInfo)persistentMemberInfo).CollectionTypeFullName);
            throw new NotImplementedException(persistentMemberInfo.GetType().FullName);
        }

        static string CleanFullName(string fullName) {
            var list = (fullName + "").Split('.').ToList();
            var name = list.Last();
            list.Remove(name);
            name = CleanName(name);
            list.Add(name);
            return list.Aggregate<string, string>(null, (current, l) => current + (l + ".")).TrimEnd('.');
        }

        static string GetCorePropertyTypeCode(IPersistentMemberInfo persistentMemberInfo) {
            DBColumnType dbColumnType = ((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType;
            switch (dbColumnType) {
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

        public static string GenerateCode(IPersistentAttributeCreator persistentAttributeCreator) {
            AttributeInfoAttribute attributeInfoAttribute = persistentAttributeCreator.Create();
            var attribute = (Attribute)ReflectionHelper.CreateObject(attributeInfoAttribute.Constructor.DeclaringType, attributeInfoAttribute.InitializedArgumentValues);
            Func<object, object> argSelector = GetArgumentCode;
            string args = attributeInfoAttribute.InitializedArgumentValues.Length > 0
                              ? attributeInfoAttribute.InitializedArgumentValues.Select(argSelector).Aggregate
                              <object, string>(null, (current, o) => current + (o + ",")).TrimEnd(',')
                              : null;
            string assemblyDecleration = null;
            if (persistentAttributeCreator is IPersistentAssemblyAttributeInfo) {
                assemblyDecleration = "assembly: ";
                if (persistentAttributeCreator is IPersistentAssemblyVersionAttributeInfo) {
                    ////args = CalculateVersion(args);
                }
            }
            string properties = GetPropertiesCode(attributeInfoAttribute);
            return string.Format("[{0}{1}({2}{3})]", assemblyDecleration, attribute.GetType().FullName, args, properties);
        }

        static string GetPropertiesCode(AttributeInfoAttribute attributeInfoAttribute) {
            if (attributeInfoAttribute.Instance == null)
                return null;
            var typeInfo = XafTypesInfo.CastTypeToTypeInfo(attributeInfoAttribute.Instance.GetType());
            var memberInfos = typeInfo.Members.Where(info => info.FindAttribute<AttributeInfoAttribute>() != null);
            Func<string, IMemberInfo, string> func = (current, memberInfo)
                => current + (memberInfo.Name + "=" + GetArgumentCodeCore(memberInfo.MemberType, memberInfo.GetValue(attributeInfoAttribute.Instance)) + ",");
            string code = memberInfos.Aggregate(null, func).TrimEnd(',');
            return string.IsNullOrEmpty(code) ? null : string.Format(",{0}", code);
        }

        private static object GetArgumentCodeCore(Type type, object argumentValue) {
            if (type == typeof(string))
                return @"@""" + argumentValue + @"""";
            if (typeof(Type).IsAssignableFrom(type))
                return "typeof(" + ((Type)argumentValue).FullName + ")";
            if (typeof(Enum).IsAssignableFrom(type))
                return argumentValue.GetType().FullName + "." + argumentValue;
            if (type == typeof(bool))
                return argumentValue.ToString().ToLower();
            return argumentValue;
        }
        /*
            static string CalculateVersion(string args) {
                args = args.Replace(@"""", "").Replace("@","");
                var version = new Version(args+".0.0");
                var totalMinutes = (int)(DateTime.Now-DateTime.Today).TotalMinutes;
                version = new Version(version.Major, version.Minor, DateTime.Today.DayOfYear, totalMinutes);
                args = version.ToString();
                return @""""+args+@"""";
            }
    */

        static object GetArgumentCode(object argumentValue) {
            return argumentValue != null ? GetArgumentCodeCore(argumentValue.GetType(), argumentValue) : null;
        }

        static string GetAssemblyAttributesCode(IPersistentAssemblyInfo persistentAssemblyInfo) {
            string code = persistentAssemblyInfo.Attributes.Aggregate("", (current, assemblyAttributeInfo) => current + (GenerateCode(assemblyAttributeInfo) + Environment.NewLine));
            return code.TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string GenerateCode(IPersistentAssemblyInfo persistentAssemblyInfo) {

            var usingsDictionary = new Dictionary<string, string>();
            string generateAssemblyCode = GetAssemblyAttributesCode(persistentAssemblyInfo) + Environment.NewLine +
                                  GetModuleCode(persistentAssemblyInfo.Name) + Environment.NewLine;
            var generatedClassCode = new StringBuilder();
            var generatedUsings = new StringBuilder();
            foreach (var persistentClassInfo in persistentAssemblyInfo.PersistentClassInfos) {
                string code = GenerateCode(persistentClassInfo);
                string usingsString = GetUsingsString(persistentAssemblyInfo, code);
                code = code.Replace(usingsString, "");
                code = code.Replace("\n", "\r\n");
                if (!(usingsDictionary.ContainsKey(usingsString))) {
                    usingsDictionary.Add(usingsString, null);
                    usingsString = usingsString.Replace("\n", "\r\n");
                    generatedUsings.Append(usingsString);
                }
                generatedClassCode.Append(code);
            }
            return generatedUsings + generateAssemblyCode + generatedClassCode;
        }

        static string GetUsingsString(IPersistentAssemblyInfo persistentAssemblyInfo, string code) {
            var regex = new Regex(persistentAssemblyInfo.CodeDomProvider == CodeDomProvider.CSharp ? "(using [^;]*;\r\n)" : "(Imports [^\r\n]*\r\n)", RegexOptions.IgnorePatternWhitespace);
            return regex.Matches(code).OfType<Match>().Aggregate("", (current, match) => current + match.Value + "\n");
        }

        internal static string GetModuleCode(string assemblyName) {
            return "namespace " + assemblyName + "{public class Dynamic" + (assemblyName + "").Replace(".", "") + "Module:DevExpress.ExpressApp.ModuleBase{}}";
        }

        public static string GetCode(IPersistentReferenceMemberInfo key) {
            var referenceMemberInfos = key.ReferenceClassInfo.OwnMembers.OfType<IPersistentReferenceMemberInfo>();
            string ret = null;
            foreach (var referenceMemberInfo in referenceMemberInfos) {
                string refPropertyName = CleanName(key.Name) + "." + CleanName(referenceMemberInfo.Name);
                var persistentMemberInfo = referenceMemberInfo.ReferenceClassInfo.OwnMembers.Where(info => info.TypeAttributes.OfType<IPersistentKeyAttribute>().Count() > 0).SingleOrDefault();
                if (persistentMemberInfo != null) {
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
            foreach (var key in keys) {
                var templateInfos = key.Owner.TemplateInfos.Where(templateInfoPredicate);
                ITemplateInfo templateInfo = templateInfos.Single();
                templateInfo.TemplateCode = @"protected override void OnLoaded() {
                                                base.OnLoaded();
                                                " + GetCode(key) + @"
                                            }";

            }

        }
        public static void SupportCompositeKeyPersistentObjects(IPersistentAssemblyInfo persistentAssemblyInfo) {
            var keys = persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers).OfType<IPersistentReferenceMemberInfo>().Where(memberInfo => memberInfo.ReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Struct);
            var dictionary = new Dictionary<IPersistentClassInfo, ITemplateInfo>();
            var objectSpace = ObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
            foreach (var key in keys) {
                if (!dictionary.ContainsKey(key.Owner)) {
                    var info = objectSpace.CreateObjectFromInterface<ITemplateInfo>();
                    info.Name = SupportPersistentObjectsAsAPartOfACompositeKey;
                    key.Owner.TemplateInfos.Add(info);
                    dictionary.Add(key.Owner, info);
                }
                ITemplateInfo templateInfo = dictionary[key.Owner];
                templateInfo.TemplateCode = @"protected override void OnLoaded() {
                                                base.OnLoaded();
                                                " + GetCode(key) + @"
                                            }";


            }

        }
    }
}