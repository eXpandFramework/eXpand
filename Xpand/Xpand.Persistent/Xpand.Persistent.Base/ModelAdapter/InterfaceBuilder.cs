﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.ModelAdapter {
    public class InterfaceBuilderData {
        readonly Type _componentType;
        readonly IEnumerable<Type> _referenceTypes = new List<Type>();

        public InterfaceBuilderData(Type componentType) {
            _componentType = componentType;
            BaseInterface = typeof(IModelNodeEnabled);
        }

        public Type ComponentType {
            get { return _componentType; }
        }

        public Func<DynamicModelPropertyInfo, bool> Act { get; set; }

        [Description("The interface from which all autogenerated interfaces derive. Default is the IModelNodeEnabled")]
        public Type BaseInterface { get; set; }

        public IEnumerable<Type> ReferenceTypes {
            get {
                return _referenceTypes;
            }
        }

        public Type RootBaseInterface { get; set; }

        public bool IsAbstract { get; set; }
    }

    public class InterfaceBuilder {
        readonly ModelInterfaceExtenders _extenders;
        private const string NamePrefix = "IModel";
        readonly List<Type> _usingTypes;
        readonly ReferencesCollector _referencesCollector;
        readonly List<StringBuilder> _builders;
        Dictionary<Type, string> _createdInterfaces;
        string _assemblyName;
        static bool _loadFromPath;
        static bool _fileExistInPath;
        static readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
        public InterfaceBuilder(ModelInterfaceExtenders extenders)
            : this() {
            _extenders = extenders;
        }

        InterfaceBuilder() {
            _usingTypes = new List<Type>();
            _builders = new List<StringBuilder>();
            _referencesCollector = new ReferencesCollector();
        }

        public static bool RuntimeMode {
            get {
                var devProcceses = new[] { "Xpand.ExpressApp.ModelEditor", "devenv" };
                return !devProcceses.Contains(Process.GetCurrentProcess().ProcessName) && LicenseManager.UsageMode != LicenseUsageMode.Designtime;
            }
        }

        public static bool LoadFromCurrentDomain { get; set; }
        public static bool LoadFromPath {
            get { return !Debugger.IsAttached && RuntimeMode && _fileExistInPath || _loadFromPath; }
            set { _loadFromPath = value; }
        }

        public Assembly Build(IEnumerable<InterfaceBuilderData> builderDatas, string assemblyFilePath = null) {
            if (string.IsNullOrEmpty(assemblyFilePath))
                assemblyFilePath = AssemblyFilePath();

            _assemblyName = Path.GetFileNameWithoutExtension(assemblyFilePath) + "";
            _createdInterfaces = new Dictionary<Type, string>();
            var source = string.Join(Environment.NewLine, new[] { GetAssemblyVersionCode(), GetCode(builderDatas) });
            _usingTypes.Add(typeof(XafApplication));
            _referencesCollector.Add(_usingTypes);
            string[] references = _referencesCollector.References.ToArray();
            if (LoadFromCurrentDomain)
                return LoadFromDomain(assemblyFilePath);
            _fileExistInPath = File.Exists(assemblyFilePath);
            if (LoadFromPath && _fileExistInPath)
                return Assembly.LoadFile(assemblyFilePath);
            if (!RuntimeMode && _assemblies.ContainsKey(_assemblyName + "")) {
                return _assemblies[_assemblyName];
            }
            var compileAssemblyFromSource = CompileAssemblyFromSource(source, references, false, assemblyFilePath);
            _assemblies.Add(_assemblyName + "", compileAssemblyFromSource);
            return compileAssemblyFromSource;
        }

        Assembly LoadFromDomain(string assemblyFilePath) {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string fileName = Path.GetFileName(assemblyFilePath);
            foreach (var assembly in assemblies) {
                if (!(assembly.IsDynamic()) && Path.GetFileName(assembly.Location) == fileName) {
                    return assembly;
                }
            }
            throw new NotImplementedException(assemblyFilePath);
        }

        private static Assembly CompileAssemblyFromSource(String source, String[] references, Boolean isDebug, String assemblyFile) {
            if (!String.IsNullOrEmpty(assemblyFile)) {
                var directoryName = Path.GetDirectoryName(assemblyFile) + "";
                if (!Directory.Exists(directoryName)) {
                    Directory.CreateDirectory(directoryName);
                }
            }
            CompilerParameters compilerParameters = GetCompilerParameters(references, isDebug, assemblyFile);
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, new[] { source });
            if (compilerResults.Errors.Count > 0) {
                RaiseCompilerException(source, compilerResults);
            }
            return compilerResults.CompiledAssembly;
        }
        private static void RaiseCompilerException(String source, CompilerResults compilerResults) {
            var errors = compilerResults.Errors.Cast<CompilerError>().Aggregate(String.Empty, (current, compilerError) => current + String.Format("({0},{1}): {2}\r\n", compilerError.Line, compilerError.Column, compilerError.ErrorText));
            throw new CompilerErrorException(compilerResults, source, errors);
        }

        private static CompilerParameters GetCompilerParameters(String[] references, Boolean isDebug, String assemblyFile) {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.AddRange(references);
            if (String.IsNullOrEmpty(assemblyFile)) {
                compilerParameters.GenerateInMemory = !isDebug;
            } else {
                compilerParameters.OutputAssembly = RuntimeMode ? assemblyFile : null;
            }
            if (isDebug) {
                compilerParameters.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
                compilerParameters.IncludeDebugInformation = true;
            }
            return compilerParameters;
        }

        string GetAssemblyVersionCode() {
            var entryAssembly = Assembly.GetEntryAssembly();
            var designTime = entryAssembly == null;
            return !designTime ? string.Format(@"[assembly: {1}(""{0}"")]", ReflectionHelper.GetAssemblyVersion(entryAssembly), TypeToString(typeof(AssemblyVersionAttribute))) : null;
        }

        string AssemblyFilePath() {
            return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) + "", GetType().Name + "_" + ".dll");
        }

        string GetCode(IEnumerable<InterfaceBuilderData> builderDatas) {
            foreach (var data in builderDatas) {
                _usingTypes.AddRange(data.ReferenceTypes);
                CreateInterface(data.ComponentType, data.Act, data.BaseInterface, data.IsAbstract, data.RootBaseInterface);
            }
            string usings = _referencesCollector.Usings.Aggregate<string, string>(null, (current, @using) => current + ("using " + @using + ";" + Environment.NewLine));
            return usings + Environment.NewLine + string.Join(Environment.NewLine, _builders.Select(builder => builder.ToString()).ToArray());
        }

        void CreateInterface(Type type, Func<DynamicModelPropertyInfo, bool> act, Type baseInterface, bool isAbstract, Type rootBaseInterface, string namePrefix = null) {
            var stringBuilder = new StringBuilder();
            var interfaceName = GetInterfaceName(_assemblyName, type, namePrefix);
            var classDecleration = ClassDecleration(type, baseInterface, isAbstract, rootBaseInterface, namePrefix);
            stringBuilder.AppendLine(classDecleration);
            var generatedInfos = new HashSet<string>();
            foreach (var propertyInfo in GetPropertyInfos(type, act)) {
                if (!generatedInfos.Contains(propertyInfo.Name)) {
                    var propertyCode = GetPropertyCode(propertyInfo, act, baseInterface);
                    stringBuilder.AppendLine(propertyCode);
                    generatedInfos.Add(propertyInfo.Name);
                }
            }
            stringBuilder.AppendLine("}");

            _createdInterfaces.Add(type, interfaceName);
            _builders.Add(stringBuilder);
        }

        string ClassDecleration(Type type, Type baseInterface, bool isAbstract, Type rootBaseInterface, string namePrefix) {
            var classDecleration = string.Join(Environment.NewLine, new[]{
                AttributeLocator(type), ModelAbstract(isAbstract),
                ClassDeclarationCore(type, baseInterface, rootBaseInterface, namePrefix)
            }
                );
            return classDecleration;
        }

        string AttributeLocator(Type type) {
            var classNameAttribute = new ClassNameAttribute(type.FullName);
            return string.Format(@"[{0}(""{1}"")]", TypeToString(classNameAttribute.GetType()), type.FullName);
        }
        [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
        public class ClassNameAttribute : Attribute {
            readonly string _typeName;

            public ClassNameAttribute(string typeName) {
                _typeName = typeName;
            }

            public string TypeName {
                get { return _typeName; }
            }
        }
        string ModelAbstract(bool isAbstract) {
            return isAbstract ? string.Format("[{0}()]", TypeToString(typeof(ModelAbstractClassAttribute))) : null;
        }

        string ClassDeclarationCore(Type type, Type baseInterface, Type rootBaseInterface, string namePrefix) {
            var interfaceName = GetInterfaceName(_assemblyName, type, namePrefix);
            var args = GetBaseInterface(baseInterface, rootBaseInterface, namePrefix == null);
            return string.Format("public interface {0}:{1}{{", interfaceName, args);
        }

        string GetBaseInterface(Type baseInterface, Type rootBaseInterface, bool isRoot) {
            return isRoot && rootBaseInterface != null ? TypeToString(rootBaseInterface) : TypeToString(baseInterface);
        }

        IEnumerable<DynamicModelPropertyInfo> GetPropertyInfos(Type type, Func<DynamicModelPropertyInfo, bool> act) {
            var propertyInfos = type.GetValidProperties().Select(DynamicModelPropertyInfo);
            return act != null ? propertyInfos.Where(act) : propertyInfos;
        }

        DynamicModelPropertyInfo DynamicModelPropertyInfo(PropertyInfo info) {
            return new DynamicModelPropertyInfo(info.Name, info.PropertyType, info.DeclaringType, info.CanRead, info.CanWrite, info);
        }

        static string GetInterfaceName(string assemblyName, Type type, string namePrefix = null) {
            var interfaceName = string.Format("{0}" + assemblyName + "{1}", namePrefix ?? NamePrefix, type.Name);
            return interfaceName;
        }

        string GetPropertyCode(DynamicModelPropertyInfo property, Func<DynamicModelPropertyInfo, bool> filter, Type baseInterface) {
            var setMethod = GetSetMethod(property);

            string interfaceName = null;
            if (setMethod == null) {
                var reflectedType = property.ReflectedType;
                interfaceName = GetInterfaceName(_assemblyName, reflectedType);
                if (!_createdInterfaces.ContainsKey(property.PropertyType))
                    CreateInterface(property.PropertyType, filter, baseInterface, false, null, interfaceName);
            }

            var propertyCode = string.Format("    {0} {1} {{ get; {2} }}", GetPropertyTypeCode(property, interfaceName), property.Name, setMethod);
            return GetAttributesCode(property) + propertyCode;
        }


        string GetPropertyTypeCode(DynamicModelPropertyInfo property, string prefix = null) {
            if (property.GetCustomAttributes(typeof(NullValueAttribute), false).Any()) {
                return TypeToString(property.PropertyType);
            }
            if (!property.PropertyType.BehaveLikeValueType()) {
                if (_createdInterfaces.ContainsKey(property.PropertyType))
                    return _createdInterfaces[property.PropertyType];
                return GetInterfaceName(_assemblyName, property.PropertyType, prefix);
            }
            Type propertyType = property.PropertyType;
            if (propertyType != typeof(string) && !propertyType.IsStruct())
                propertyType = typeof(Nullable<>).MakeNullAble(property.PropertyType);
            return TypeToString(propertyType);
        }

        object GetSetMethod(DynamicModelPropertyInfo property) {
            return property.PropertyType.BehaveLikeValueType() ? "set;" : null;
        }

        string GetAttributesCode(DynamicModelPropertyInfo property) {
            var attributes = property.GetCustomAttributes(false).OfType<Attribute>().ToList();
            if (property.PropertyType == typeof(string) && !attributes.OfType<LocalizableAttribute>().Any()) {
                attributes.Add(new LocalizableAttribute(true));
            }
            IEnumerable<string> codeList = attributes.Select(attribute => GetAttributeCode(attribute, property)).Where(attributeCode => !string.IsNullOrEmpty(attributeCode));
            return codeList.Aggregate<string, string>(null, (current, attributeCode) => current + string.Format("   [{0}]{1}", attributeCode, Environment.NewLine));
        }

        string GetAttributeCode(object attribute, DynamicModelPropertyInfo info) {
            if (attribute == null) {
                return string.Empty;
            }
            var localizableAttribute = attribute as LocalizableAttribute;
            if (localizableAttribute != null && info.PropertyType == typeof(string)) {
                string arg = (localizableAttribute).IsLocalizable.ToString(CultureInfo.InvariantCulture).ToLower();
                return string.Format("{1}({0})", arg, TypeToString(attribute.GetType()));
            }
            if (attribute.GetType() == typeof(CategoryAttribute)) {
                string arg = ((CategoryAttribute)attribute).Category;
                return string.Format(@"{1}(""{0}"")", arg, TypeToString(attribute.GetType()));
            }
            if (attribute is RequiredAttribute) {
                return string.Format(@"{0}()", TypeToString(attribute.GetType()));
            }
            var editorAttribute = attribute as EditorAttribute;
            if (editorAttribute != null) {
                string arg1 = (editorAttribute).EditorTypeName;
                string arg2 = (editorAttribute).EditorBaseTypeName;
                return string.Format(@"{2}(""{0}"", ""{1}"")", arg1, arg2, TypeToString(attribute.GetType()));
            }
            var refreshPropertiesAttribute = attribute as RefreshPropertiesAttribute;
            if (refreshPropertiesAttribute != null) {
                string arg = TypeToString(refreshPropertiesAttribute.RefreshProperties.GetType()) + "." + refreshPropertiesAttribute.RefreshProperties.ToString();
                return string.Format(@"{1}({0})", arg, TypeToString(attribute.GetType()));
            }
            var typeConverterAttribute = attribute as TypeConverterAttribute;
            if (typeConverterAttribute != null) {
                var type = Type.GetType((typeConverterAttribute).ConverterTypeName);
                if (type != null && type.IsPublic && !type.FullName.Contains(".Design."))
                    return string.Format("{1}(typeof({0}))", type.FullName, TypeToString(attribute.GetType()));
                return null;
            }
            Type attributeType = attribute.GetType();
            if (attributeType == typeof(DXDescriptionAttribute)) {
                string description = ((DXDescriptionAttribute)attribute).Description.Replace(@"""", @"""""");
                return string.Format(@"{1}(@""{0}"")", description, TypeToString(typeof(DescriptionAttribute)));
            }
            if (typeof(DescriptionAttribute).IsAssignableFrom(attributeType)) {
                string description = ((DescriptionAttribute)attribute).Description.Replace(@"""", @"""""");
                return string.Format(@"{1}(@""{0}"")", description, TypeToString(typeof(DescriptionAttribute)));
            }
            if (attributeType == typeof(DefaultValueAttribute)) {
                string value = GetStringValue(((DefaultValueAttribute)attribute).Value);
                return string.Format(@"System.ComponentModel.DefaultValue({0})", value);
            }
            if (attributeType == typeof(ModelValueCalculatorWrapperAttribute)) {
                var modelValueCalculatorAttribute = ((ModelValueCalculatorWrapperAttribute)attribute);
                if (modelValueCalculatorAttribute.CalculatorType != null) {
                    return string.Format(@"{0}(typeof({1}))", TypeToString(typeof(ModelValueCalculatorAttribute)), TypeToString(modelValueCalculatorAttribute.CalculatorType));
                }
                var linkValue = GetStringValue(modelValueCalculatorAttribute.LinkValue);
                return string.Format(@"{0}({1})", TypeToString(typeof(ModelValueCalculatorAttribute)), linkValue);
            }
            if (attributeType == typeof(ModelReadOnlyAttribute)) {
                var readOnlyCalculatorType = ((ModelReadOnlyAttribute)attribute).ReadOnlyCalculatorType;
                return string.Format(@"{0}(typeof({1}))", TypeToString(typeof(ModelReadOnlyAttribute)), TypeToString(readOnlyCalculatorType));
            }
            if (attributeType == typeof(ReadOnlyAttribute)) {
                string value = GetStringValue(((ReadOnlyAttribute)attribute).IsReadOnly);
                return string.Format(@"{1}({0})", value, TypeToString(attribute.GetType()));
            }
            return string.Empty;
        }
        string GetStringValue(object value) {
            return GetStringValue(value != null ? value.GetType() : null, value);
        }
        string GetStringValue(Type type, object value) {
            if (type == null || value == null) {
                return "null";
            }
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null) {
                type = nullableType;
            }
            if (type == typeof(string)) {
                return string.Format(@"@""{0}""", ((string)value).Replace("\"", "\"\""));
            }
            if (type == typeof(Boolean)) {
                return (bool)value ? "true" : "false";
            }
            if (type.IsEnum) {
                if (value is int) {
                    value = Enum.ToObject(type, (int)value);
                }
                return string.Format("{0}.{1}", TypeToString(type), value);
            }
            if (type.IsClass) {
                return string.Format("typeof({0})", TypeToString(value.GetType()));
            }
            if (type == typeof(char)) {
                var args = Convert.ToString(value);
                if (args == "\0")
                    return @"""""";
                throw new NotImplementedException();
            }
            return string.Format(CultureInfo.InvariantCulture.NumberFormat, "({0})({1})", TypeToString(type), value);
        }
        string TypeToString(Type type) {
            return HelperTypeGenerator.TypeToString(type, _usingTypes, true);
        }

        public void ExtendInteface(Type targetType, Type extenderType, Assembly assembly) {
            extenderType = GalcType(extenderType, assembly);
            targetType = GalcType(targetType, assembly);
            _extenders.Add(targetType, extenderType);
        }

        Type GalcType(Type extenderType, Assembly assembly) {
            if (!extenderType.IsInterface) {
                var type = assembly.GetTypes().SingleOrDefault(type1 => AttributeLocatorMatches(extenderType, type1));
                if (type == null) {
                    Debugger.Break();
                    throw new NullReferenceException("Cannot locate the dynamic interface for " + extenderType.FullName);
                }
                return type;
            }
            return extenderType;
        }

        bool AttributeLocatorMatches(Type extenderType, Type type1) {
            return type1.GetCustomAttributes(typeof(Attribute), false).Any(attribute => AttributeMatch(extenderType, (Attribute)attribute));
        }

        bool AttributeMatch(Type extenderType, Attribute attribute) {
            Type type = attribute.GetType();
            if (type.Name == typeof(ClassNameAttribute).Name) {
                var value = type.GetProperty("TypeName", BindingFlags.Instance | BindingFlags.Public).GetValue(attribute, null) + "";
                value = value.Substring(1 + value.LastIndexOf(".", StringComparison.Ordinal));
                return extenderType.Name == value;
            }
            return false;
        }

        public void ExtendInteface<TTargetInterface, TComponent>(Assembly assembly) where TComponent : class {
            ExtendInteface(typeof(TTargetInterface), typeof(TComponent), assembly);
        }

        public static string GeneratedEmptyInterfacesCode(IEnumerable<ITypeInfo> typeInfos, Type baseType, Func<ITypeInfo, Type, string, string> func = null) {
            return typeInfos.Aggregate<ITypeInfo, string>(null, (current, typeInfo) => current + (GenerateInterfaceCode(typeInfo, baseType, func) + Environment.NewLine)).TrimEnd(Environment.NewLine.ToCharArray());
        }

        static string GenerateInterfaceCode(ITypeInfo typeInfo, Type baseType, Func<ITypeInfo, Type, string, string> func) {
            var interfaceBuilder = new InterfaceBuilder();
            var classDeclaration = interfaceBuilder.ClassDeclarationCore(typeInfo.Type, baseType, null, null);
            string code = null;
            if (func != null)
                code = func.Invoke(typeInfo, baseType, GetInterfaceName(null, typeInfo.Type));

            var interfaceCode = code + Environment.NewLine + classDeclaration + Environment.NewLine + "}";
            return interfaceCode;
        }

        public static string GeneratedDisplayNameCode(string arg3) {
            var interfaceBuilder = new InterfaceBuilder();
            return string.Format(@"[{0}(""{1}"")]", interfaceBuilder.TypeToString(typeof(ModelDisplayNameAttribute)), arg3);
        }
    }

    public class ModelValueCalculatorWrapperAttribute : Attribute {
        readonly Type _calculatorType;
        readonly string _linkValue;
        readonly string _nodeName;
        readonly string _nodeTypeName;
        readonly string _propertyName;

        public ModelValueCalculatorWrapperAttribute(Type calculatorType) {
            _calculatorType = calculatorType;

        }

        public ModelValueCalculatorWrapperAttribute(ModelValueCalculatorAttribute modelValueCalculatorAttribute, Type calculatorType)
            : this(calculatorType) {
            _linkValue = modelValueCalculatorAttribute.LinkValue;
            _nodeName = modelValueCalculatorAttribute.NodeName;
            if (modelValueCalculatorAttribute.NodeType != null)
                _nodeTypeName = modelValueCalculatorAttribute.NodeType.Name;
            _propertyName = modelValueCalculatorAttribute.PropertyName;
        }

        public Type CalculatorType {
            get { return _calculatorType; }
        }

        public string LinkValue {
            get { return _linkValue; }
        }

        public string NodeName {
            get { return _nodeName; }
        }

        public string NodeTypeName {
            get { return _nodeTypeName; }
        }

        public string PropertyName {
            get { return _propertyName; }
        }
    }
    public static class InterfaceBuilderExtensions {

        public static Type MakeNullAble(this Type generic, params Type[] args) {
            return args[0].IsNullableType() ? args[0] : typeof(Nullable<>).MakeGenericType(args);
        }

        public static PropertyInfo[] GetPublicProperties(this Type type) {
            if (type.IsInterface) {
                var propertyInfos = new List<PropertyInfo>();
                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0) {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces()) {
                        if (considered.Contains(subInterface)) continue;
                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }
                    var typeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                    var newPropertyInfos = typeProperties.Where(x => !propertyInfos.Contains(x));
                    propertyInfos.InsertRange(0, newPropertyInfos);
                }
                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        public static bool FilterAttributes(this DynamicModelPropertyInfo info, Type[] attributes) {
            return attributes.SelectMany(type => info.GetCustomAttributes(type, false)).Any();
        }

        public static bool DXFilter(this DynamicModelPropertyInfo info, Type[] attributes = null) {
            return info.DXFilter(info.DeclaringType, attributes);
        }

        public static readonly IList<Type> BaseTypes = new List<Type> { typeof(BaseOptions), typeof(FormatInfo), typeof(AppearanceObject), typeof(TextOptions) };
        public static bool DXFilter(this DynamicModelPropertyInfo info, Type componentBaseType, Type[] attributes = null) {
            return DXFilter(info, BaseTypes, componentBaseType, attributes);
        }
        public static bool DXFilter(this DynamicModelPropertyInfo info, IList<Type> baseTypes, Type componentBaseType, Type[] attributes = null) {
            if (attributes == null)
                attributes = new[] { typeof(XtraSerializableProperty) };
            return Filter(info, componentBaseType, BaseTypes.Union(baseTypes).ToArray(), attributes);
        }

        public static bool Filter(this DynamicModelPropertyInfo info, Type componentBaseType, Type[] filteredPropertyBaseTypes, Type[] attributes) {
            return info.IsBrowseable() && info.HasAttributes(attributes) &&
                   FilterCore(info, componentBaseType, filteredPropertyBaseTypes);
        }

        static bool FilterCore(DynamicModelPropertyInfo info, Type componentBaseType, IEnumerable<Type> filteredPropertyBaseTypes) {
            var behaveLikeValueType = info.PropertyType.BehaveLikeValueType();
            var isBaseViewProperty = componentBaseType.IsAssignableFrom(info.DeclaringType);
            var filterCore = filteredPropertyBaseTypes.Any(type => type.IsAssignableFrom(info.PropertyType)) || behaveLikeValueType;
            var core = filteredPropertyBaseTypes.Any(type => type.IsAssignableFrom(info.DeclaringType)) && behaveLikeValueType;
            return isBaseViewProperty ? filterCore : core;
        }

        public static bool IsValidEnum(this Type propertyType, object value) {
            return !propertyType.IsEnum || Enum.IsDefined(value.GetType(), value);
        }

        public static bool IsNullableType(this Type theType) {
            if (theType.IsGenericType) {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null) return (genericTypeDefinition == typeof(Nullable<>));
            }
            return false;
        }


        public static bool IsStruct(this Type type) {
            if (type.IsNullableType())
                type = type.GetGenericArguments()[0];
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        public static bool IsBrowseable(this PropertyInfo propertyInfo) {
            return propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), false).OfType<BrowsableAttribute>().All(o => o.Browsable);
        }

        public static bool BehaveLikeValueType(this Type type) {
            return type == typeof(string) || type.IsValueType;
        }

        public static IEnumerable<PropertyInfo> GetValidProperties(this Type type, params Type[] attributes) {
            if (type != null) {
                var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Distinct(new PropertyInfoEqualityComparer());
                var infos = propertyInfos.Where(info => HasAttributes(info, attributes));
                return infos.Where(IsValidProperty).ToArray();
            }
            return new PropertyInfo[0];
        }

        static bool HasAttributes(this PropertyInfo propertyInfo, params Type[] attributes) {
            return (attributes == null || attributes == Type.EmptyTypes) || (attributes.Any()) || (attributes.All(type => propertyInfo.GetCustomAttributes(type, false).Any()));
        }

        class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo> {
            public bool Equals(PropertyInfo x, PropertyInfo y) {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(PropertyInfo obj) {
                return obj.Name.GetHashCode();
            }
        }

        static bool IsValidProperty(PropertyInfo info) {
            if (IsObsolete(info))
                return false;
            return !info.PropertyType.BehaveLikeValueType() || info.GetSetMethod() != null && info.GetGetMethod() != null;
        }
        static bool IsObsolete(PropertyInfo info) {
            return info.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0;
        }
        public static void SetBrowsable(this DynamicModelPropertyInfo info, Dictionary<string, bool> propertyNames) {
            if (propertyNames.ContainsKey(info.Name)) {
                info.RemoveAttributes(typeof(BrowsableAttribute));
                info.AddAttribute(new BrowsableAttribute(propertyNames[info.Name]));
            }
        }
        public static void SetCategory(this DynamicModelPropertyInfo info, Dictionary<string, string> propertyNames) {
            if (propertyNames.ContainsKey(info.Name)) {
                info.RemoveAttributes(typeof(BrowsableAttribute));
                info.AddAttribute(new CategoryAttribute(propertyNames[info.Name]));
            }
        }

        public static void CreateValueCalculator(this DynamicModelPropertyInfo info, IModelValueCalculator modelValueCalculator) {
            CreateValueCalculatorCore(info);
            info.AddAttribute(new ModelValueCalculatorWrapperAttribute(modelValueCalculator.GetType()));
        }

        public static void CreateValueCalculator(this DynamicModelPropertyInfo info, string expressionPath = null) {
            CreateValueCalculatorCore(info);
            ModelValueCalculatorAttribute modelValueCalculatorAttribute;
            if (expressionPath != null) {
                modelValueCalculatorAttribute = new ModelValueCalculatorAttribute(expressionPath);
                info.AddAttribute(new ModelValueCalculatorWrapperAttribute(modelValueCalculatorAttribute, null));
            } else {
                info.RemoveAttributes(typeof(ReadOnlyAttribute));
                var type = typeof(MapModelValueCalculator);
                modelValueCalculatorAttribute = new ModelValueCalculatorAttribute(type);
                info.AddAttribute(new ModelValueCalculatorWrapperAttribute(modelValueCalculatorAttribute, type));
                info.AddAttribute(new ModelReadOnlyAttribute(typeof(MapModelReadOnlyCalculator)));
            }
        }

        static void CreateValueCalculatorCore(DynamicModelPropertyInfo info) {
            info.RemoveAttributes(typeof(DefaultValueAttribute));
            info.AddAttribute(new ReadOnlyAttribute(true));
        }

        public static void SetDefaultValues(this DynamicModelPropertyInfo info, Dictionary<string, object> propertyNames) {
            if (propertyNames.ContainsKey(info.Name)) {
                info.RemoveAttributes(typeof(DefaultValueAttribute));
                info.AddAttribute(new DefaultValueAttribute(propertyNames[info.Name]));
            }
        }


    }

    public sealed class DynamicModelPropertyInfo : PropertyInfo {
        readonly List<object> _attributesCore = new List<object>();
        readonly bool _canReadCore;
        readonly bool _canWriteCore;
        readonly PropertyInfo _targetPropertyInfo;
        readonly Type _declaringTypeCore;
        readonly string _nameCore;
        readonly Type _propertyTypeCore;

        public DynamicModelPropertyInfo(string name, Type propertyType, Type declaringType, bool canRead, bool canWrite, PropertyInfo targetPropertyInfo) {
            _nameCore = name;
            _propertyTypeCore = propertyType;
            _declaringTypeCore = declaringType;
            _canReadCore = canRead;
            _canWriteCore = canWrite;
            _targetPropertyInfo = targetPropertyInfo;
            _attributesCore.AddRange(targetPropertyInfo.GetCustomAttributes(false).Where(o => !(o is DefaultValueAttribute)));
        }


        public override string Name {
            get { return _nameCore; }
        }

        public override Type PropertyType {
            get { return _propertyTypeCore; }
        }

        public override Type DeclaringType {
            get { return _declaringTypeCore; }
        }

        public override bool CanRead {
            get { return _canReadCore; }
        }

        public override bool CanWrite {
            get { return _canWriteCore; }
        }

        public override PropertyAttributes Attributes {
            get { throw new NotImplementedException(); }
        }

        public override Type ReflectedType {
            get { return _targetPropertyInfo.ReflectedType; }
        }

        public override string ToString() {
            return _targetPropertyInfo.ToString();
        }
        public override MethodInfo[] GetAccessors(bool nonPublic) {
            return _targetPropertyInfo.GetAccessors(nonPublic);
        }

        public override MethodInfo GetGetMethod(bool nonPublic) {
            return _targetPropertyInfo.GetSetMethod(nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters() {
            return _targetPropertyInfo.GetIndexParameters();
        }

        public override MethodInfo GetSetMethod(bool nonPublic) {
            return _targetPropertyInfo.GetSetMethod(nonPublic);
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
            return _targetPropertyInfo.GetValue(obj, invokeAttr, binder, index, culture);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
            _targetPropertyInfo.SetValue(obj, value, invokeAttr, binder, index, culture);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return _attributesCore.Where(attributeType.IsInstanceOfType).ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return _attributesCore.ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return _targetPropertyInfo.IsDefined(attributeType, inherit);
        }

        public void RemoveAttribute(Attribute attribute) {
            _attributesCore.Remove(attribute);
        }

        public void AddAttribute(Attribute attribute) {
            _attributesCore.Add(attribute);
        }

        public void RemoveInvalidTypeConverterAttributes(string nameSpace) {
            var customAttributes = GetCustomAttributes(typeof(TypeConverterAttribute), false).OfType<TypeConverterAttribute>();
            var attributes = customAttributes.Where(attribute => attribute.ConverterTypeName.StartsWith(nameSpace));
            foreach (var customAttribute in attributes) {
                _attributesCore.Remove(customAttribute);
            }
        }

        public void RemoveAttributes(Type type) {
            foreach (var customAttribute in GetCustomAttributes(type, false).OfType<Attribute>()) {
                _attributesCore.Remove(customAttribute);
            }
        }
    }

}
