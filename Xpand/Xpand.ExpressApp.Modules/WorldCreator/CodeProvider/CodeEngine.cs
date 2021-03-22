using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Extensions.StringExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.CodeProvider{
    public static class CodeEngine{
        public const string SupportPersistentObjectsAsAPartOfACompositeKey =
            "SupportPersistentObjectsAsAPartOfACompositeKey";

        public static string GenerateCode(this IPersistentMemberInfo persistentMemberInfo){
            if (persistentMemberInfo.CodeTemplateInfo.TemplateInfo != null){
                string code = persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
                if (code != null){
                    code = code.Replace("$TYPEATTRIBUTES$", GetAttributesCode(persistentMemberInfo));
                    code = code.Replace("$PROPERTYNAME$", GetMemberName(persistentMemberInfo));
                    code = code.Replace("$PROPERTYTYPE$", GetPropertyTypeCode(persistentMemberInfo));
                    code = code.Replace("$INJECTCODE$", GetInjectCode(persistentMemberInfo));
                }
                return code;
            }
            return null;
        }

        private static string GetAttributesCode(IPersistentMemberInfo persistentMemberInfo){
            return string.Join(Environment.NewLine, persistentMemberInfo.TypeAttributes.Select(GenerateCode));
        }

        static string GetMemberName(IPersistentMemberInfo persistentMemberInfo){
            string memberName = persistentMemberInfo.Name.CleanCodeName();
            if (persistentMemberInfo.Owner.Name == persistentMemberInfo.Name)
                memberName += "Member";
            return memberName;
        }

        public static string GenerateCode(this IPersistentClassInfo persistentClassInfo){
            if (persistentClassInfo.CodeTemplateInfo?.TemplateInfo != null){
                string code = persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
                string attributesCode = GetAttributesCode(persistentClassInfo);
                if (code != null){
                    code = code.Replace("$ASSEMBLYNAME$", persistentClassInfo.PersistentAssemblyInfo.Name);
                    code = code.Replace("$TYPEATTRIBUTES$", attributesCode);
                    code = code.Replace("$CLASSNAME$", persistentClassInfo.Name.CleanCodeName());
                    code = code.Replace("$BASECLASSNAME$",
                        persistentClassInfo.BaseTypeFullName + GetInterfacesCode(persistentClassInfo));
                    code = code.Replace("$INJECTCODE$", GetInjectCode(persistentClassInfo));
                    code = GetAllMembersCode(persistentClassInfo, code);
                    return code;
                }
            }
            return null;
        }

        static string GetInjectCode(IPersistentTypeInfo persistentTypeInfo){
            return persistentTypeInfo.TemplateInfos.Aggregate<ITemplateInfo, string>(null,
                (current, codeTemplate) => current + (codeTemplate.TemplateCode + Environment.NewLine));
        }

        static string GetAllMembersCode(IPersistentClassInfo persistentClassInfo, string code){
            string allMemberGeneratedCode = GetMembersCode(persistentClassInfo.OwnMembers);
            int anchorsFound = 0;
            for (int i = code.Length - 1; i > -1; i--){
                if (code[i] == '}' && anchorsFound < 2)
                    anchorsFound++;
                if (anchorsFound == 2){
                    code = code.Substring(0, i) + allMemberGeneratedCode + code.Substring(i);
                    break;
                }
            }
            return code;
        }

        static string GetInterfacesCode(IPersistentClassInfo persistentClassInfo){
            return persistentClassInfo.Interfaces.Aggregate<IInterfaceInfo, string>(null,
                (current, interfaceInfo) => current + ("," + interfaceInfo.Type.FullName));
        }


        static string GetAttributesCode(IPersistentClassInfo persistentClassInfo){
            return string.Join(Environment.NewLine, persistentClassInfo.TypeAttributes.Select(GenerateCode));
        }


        static string GetMembersCode(IEnumerable<IPersistentMemberInfo> persistentMemberInfos){
            return string.Join(Environment.NewLine, persistentMemberInfos.Select(GenerateCode));
        }

        static string GetPropertyTypeCode(IPersistentMemberInfo persistentMemberInfo){
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                return GetCorePropertyTypeCode(persistentMemberInfo);
            if (persistentMemberInfo is IPersistentReferenceMemberInfo persistentReferenceMemberInfo)
                return CleanFullName((persistentReferenceMemberInfo).ReferenceTypeFullName);
            if (persistentMemberInfo is IPersistentCollectionMemberInfo persistentCollectionMemberInfo)
                return CleanFullName((persistentCollectionMemberInfo).CollectionTypeFullName);
            throw new NotImplementedException(persistentMemberInfo.GetType().FullName);
        }

        static string CleanFullName(string fullName){
            var list = (fullName + "").Split('.').ToList();
            var name = list.Last();
            list.Remove(name);
            name = name.CleanCodeName();
            list.Add(name);
            return list.Aggregate<string, string>(null, (current, l) => current + (l + ".")).TrimEnd('.');
        }

        static string GetCorePropertyTypeCode(IPersistentMemberInfo persistentMemberInfo){
            DBColumnType dbColumnType = ((IPersistentCoreTypeMemberInfo) persistentMemberInfo).DataType;
            switch (dbColumnType){
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
                case DBColumnType.ByteArray:
                    return "byte[]";
                default:
                    return dbColumnType.ToString();
            }
        }

        public static string GenerateCode(this IPersistentAttributeCreator persistentAttributeCreator){
            AttributeInfoAttribute attributeInfoAttribute = persistentAttributeCreator.Create();
            var argumentValues = attributeInfoAttribute.InitializedArgumentValues;
            var attribute =(Attribute)ReflectionHelper.CreateObject(attributeInfoAttribute.Constructor.DeclaringType, argumentValues);
            var args = argumentValues.Length > 0 ? string.Join(",", argumentValues.Select(GetArgumentCode)) : null;
            string assemblyDecleration = null;
            if (persistentAttributeCreator is IPersistentAssemblyAttributeInfo){
                assemblyDecleration = "assembly: ";
                if (persistentAttributeCreator is IPersistentAssemblyVersionAttributeInfo attributeInfo){
                    args = @"""" + attributeInfo.Owner.Version() + @"""";
                }
            }
            string properties = GetPropertiesCode(attributeInfoAttribute);
            args = (args != null && properties != null) ? args + ", " : args;
            return $"[{assemblyDecleration}{attribute.GetType().FullName}({args}{properties})]";
        }

        static string GetPropertiesCode(AttributeInfoAttribute attributeInfoAttribute){
            if (attributeInfoAttribute.Instance == null)
                return null;
            var typeInfo = XafTypesInfo.CastTypeToTypeInfo(attributeInfoAttribute.Instance.GetType());
            var memberInfos = typeInfo.Members.Where(info => info.FindAttribute<AttributeInfoAttribute>() != null);
            string Func(string current, IMemberInfo memberInfo) => current + (memberInfo.Name + "=" + GetArgumentCodeCore(memberInfo.GetValue(attributeInfoAttribute.Instance)) + ",");

            return memberInfos.Aggregate(null, (Func<string, IMemberInfo, string>) Func)?.TrimEnd(',');
        }

        private static object GetArgumentCodeCore(object argumentValue){
            if (argumentValue == null)
                return "null";
            var type = argumentValue.GetType();
            if (type == typeof(string))
                return @"@""" + argumentValue + @"""";
            if (typeof(Type).IsAssignableFrom(type))
                return "typeof(" + ((Type) argumentValue).FullName + ")";
            if (typeof(Enum).IsAssignableFrom(type))
                return type.FullName + "." + argumentValue;
            if (type == typeof(bool))
                return argumentValue.ToString().ToLower();

            return argumentValue;
        }

        static object GetArgumentCode(object argumentValue){
            return GetArgumentCodeCore(argumentValue);
        }

        static string GetAssemblyAttributesCode(IPersistentAssemblyInfo persistentAssemblyInfo){
            return string.Join(Environment.NewLine, persistentAssemblyInfo.Attributes.Select(GenerateCode));
        }

        public static string GenerateCode(this IPersistentAssemblyInfo persistentAssemblyInfo){
            if (persistentAssemblyInfo.Session.IsObjectMarkedDeleted(persistentAssemblyInfo))
                return null;
//            persistentAssemblyInfo.CreateMissingAssociations();
            var usingsDictionary = new Dictionary<string, string>();
            string generateAssemblyCode = GetAssemblyAttributesCode(persistentAssemblyInfo) + Environment.NewLine +
                                          GetModuleCode(persistentAssemblyInfo.Name) + Environment.NewLine;
            var classCode = new StringBuilder();
            var generatedUsings = new StringBuilder();
            foreach (var persistentClassInfo in persistentAssemblyInfo.PersistentClassInfos){
                string code = GenerateCode(persistentClassInfo);
                if (code != null){
                    string usingsString = GetUsingsString(persistentAssemblyInfo, code);
                    if (usingsString != string.Empty)
                        code = code.Replace(usingsString, "");
                    code = code.Replace("\n", "\r\n");
                    if (!(usingsDictionary.ContainsKey(usingsString))){
                        usingsDictionary.Add(usingsString, null);
                        usingsString = usingsString.Replace("\n", "\r\n");
                        generatedUsings.Append(usingsString);
                    }
                    classCode.Append(code);
                }
            }
            return generatedUsings + generateAssemblyCode + classCode;
        }

        static string GetUsingsString(IPersistentAssemblyInfo persistentAssemblyInfo, string code){
            var regex =
                new Regex(
                    persistentAssemblyInfo.CodeDomProvider == CodeDomProvider.CSharp
                        ? "^using .*;"
                        : "^Imports .*;", RegexOptions.Multiline);
            return regex.Matches(code).OfType<Match>().Aggregate("", (current, match) => current + match.Value + "\n");
        }

        internal static string GetModuleCode(string assemblyName){
            return "namespace " + assemblyName + "{public class Dynamic" + (assemblyName + "").Replace(".", "") +
                   "Module:DevExpress.ExpressApp.ModuleBase{}}";
        }

        static string GetCode(IPersistentReferenceMemberInfo key){
            var referenceMemberInfos = key.ReferenceClassInfo.OwnMembers.OfType<IPersistentReferenceMemberInfo>();
            string ret = null;
            foreach (var referenceMemberInfo in referenceMemberInfos){
                string refPropertyName = key.Name.CleanCodeName() + "." + referenceMemberInfo.Name.CleanCodeName();
                var persistentMemberInfo =
                    referenceMemberInfo.ReferenceClassInfo.OwnMembers.SingleOrDefault(
                        info => info.TypeAttributes.OfType<IPersistentKeyAttribute>().Any());
                if (persistentMemberInfo != null){
                    var refKeyName = persistentMemberInfo.Name.CleanCodeName();
                    ret += @"if(" + refPropertyName + ".Session != Session){" +
                           refPropertyName + "=Session.GetObjectByKey<" +
                           referenceMemberInfo.ReferenceClassInfo.Name.CleanCodeName() + ">(" + refPropertyName + "." +
                           refKeyName + ");"
                           + "}";
                }
            }
            return ret;
        }

        public static void SupportCompositeKeyPersistentObjects(this IPersistentAssemblyInfo persistentAssemblyInfo,
            Func<ITemplateInfo, bool> templateInfoPredicate){
            var keys =
                persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers)
                    .OfType<IPersistentReferenceMemberInfo>()
                    .Where(
                        memberInfo =>
                            memberInfo.ReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType ==
                            TemplateType.Struct);
            foreach (var key in keys){
                var templateInfos = key.Owner.TemplateInfos.Where(templateInfoPredicate);
                ITemplateInfo templateInfo = templateInfos.Single();
                templateInfo.TemplateCode = @"protected override void OnLoaded() {
                                                base.OnLoaded();
                                                " + GetCode(key) + @"
                                            }";
            }
        }

        public static void SupportCompositeKeyPersistentObjects(this IPersistentAssemblyInfo persistentAssemblyInfo){
            var keys =
                persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers)
                    .OfType<IPersistentReferenceMemberInfo>()
                    .Where(
                        memberInfo =>
                            memberInfo.ReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType ==
                            TemplateType.Struct);
            var dictionary = new Dictionary<IPersistentClassInfo, ITemplateInfo>();
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
            foreach (var key in keys){
                if (!dictionary.ContainsKey(key.Owner)){
                    var info = objectSpace.Create<ITemplateInfo>();
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