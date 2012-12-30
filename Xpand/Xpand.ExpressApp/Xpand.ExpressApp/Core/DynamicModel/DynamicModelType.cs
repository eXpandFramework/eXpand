﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xpand.ExpressApp.Core.DynamicModel {
    [Obsolete("", true)]
    public sealed class DynamicModelType : Type {
        readonly List<Attribute> attributesCore = new List<Attribute>();
        readonly Type baseTypeCore;
        readonly Guid guidCore;
        readonly IDynamicModelPropertiesSource sourceDynamicModelPropertiesCore;
        readonly Type _sourceTypeForDynamicModelPropertiesCore;
        readonly string _category;
        List<PropertyInfo> propertiesCore;
        readonly Func<PropertyInfo, bool> _filterPredicate;
        readonly DynamicDouplicateTypesMapper _douplicateTypesMapper;


        DynamicModelType(Type baseInterface) {
            baseTypeCore = baseInterface;
            guidCore = Guid.NewGuid();
        }

        public DynamicModelType(Type baseInterface,
                                IDynamicModelPropertiesSource sourceDynamicModelProperties)
            : this(baseInterface) {
            sourceDynamicModelPropertiesCore = sourceDynamicModelProperties;
            AfterInitSource();
        }

        public DynamicModelType(Type baseInterface, Type sourceTypeForDynamicModelProperties)
            : this(baseInterface) {
            _sourceTypeForDynamicModelPropertiesCore = sourceTypeForDynamicModelProperties;
            AfterInitSource();
        }

        public DynamicModelType(Type baseInterface, Type sourceTypeForDynamicModelPropertiesCore, string category, Func<PropertyInfo, bool> filterPredicate, DynamicDouplicateTypesMapper douplicateTypesMapper)
            : this(baseInterface) {
            _sourceTypeForDynamicModelPropertiesCore = sourceTypeForDynamicModelPropertiesCore;
            _category = category;
            _filterPredicate = filterPredicate;
            _douplicateTypesMapper = douplicateTypesMapper;
            AfterInitSource();
        }

        public override Guid GUID {
            get { return guidCore; }
        }

        public override string Name {
            get { return String.Format("IModelXpand{0}", GetSourceName()); }
        }

        public override Type BaseType {
            get { return null; }
        }

        public Type BaseTypeCore {
            get { return baseTypeCore; }
        }

        public override Assembly Assembly {
            get { return BaseTypeCore.Assembly; }
        }

        public override string AssemblyQualifiedName {
            get { return BaseTypeCore.AssemblyQualifiedName; }
        }

        public override string FullName {
            get {
                string fullName = BaseTypeCore.FullName + "";
                return fullName.Remove(fullName.Length - BaseTypeCore.Name.Length) + Name;
            }
        }

        public override Module Module {
            get { return BaseTypeCore.Module; }
        }

        public override string Namespace {
            get { return BaseTypeCore.Namespace; }
        }

        public override Type UnderlyingSystemType {
            get { return BaseTypeCore.UnderlyingSystemType; }
        }

        void AfterInitSource() {
            //            attributesCore.Add(new ModelAutoGeneratedTypeAttribute(baseTypeCore, GetSourceName(), "Xpand", "Xpand"));
            propertiesCore = new List<PropertyInfo>(GetPropertiesCore());
        }

        string GetSourceName() {
            if (sourceDynamicModelPropertiesCore != null)
                return sourceDynamicModelPropertiesCore.GetType().Name;
            if (_sourceTypeForDynamicModelPropertiesCore != null)
                return _sourceTypeForDynamicModelPropertiesCore.Name;
            throw new NullReferenceException("sourceTypeForDynamicModelPropertiesCore");
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
            if ((bindingAttr & BindingFlags.Public) != BindingFlags.Public) return new PropertyInfo[0];
            var propertyInfos = _filterPredicate != null ? propertiesCore.Where(_filterPredicate) : propertiesCore;
            return propertyInfos.Select(info => {
                Type propertyType = GetPropertyType(info);
                if (propertyType.IsValueType) {
                    propertyType = typeof(Nullable<>).MakeGenericType(new[] { propertyType });
                }
                var simplePropertyInfo = new XpandPropertyInfo(info.Name, propertyType, GetType(), info.CanRead, info.CanWrite);
                if (_category != null) simplePropertyInfo.AddAttribute(new CategoryAttribute(_category));
                return simplePropertyInfo;
            }).OfType<PropertyInfo>().ToArray();

        }

        Type GetPropertyType(PropertyInfo info) {
            return _douplicateTypesMapper != null && _douplicateTypesMapper.ContainsKey(info.PropertyType)
                       ? _douplicateTypesMapper[info.PropertyType]
                       : info.PropertyType;
        }

        IEnumerable<PropertyInfo> GetPropertiesCore() {
            var properties = new PropertyInfo[0];
            if (sourceDynamicModelPropertiesCore != null)
                properties = sourceDynamicModelPropertiesCore.GetProperties();
            if (_sourceTypeForDynamicModelPropertiesCore != null)
                properties =
                    _sourceTypeForDynamicModelPropertiesCore.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties;
        }


        public override bool Equals(object obj) {
            var type = obj as DynamicModelType;
            if (type != null)
                return type.Name == Name && type.BaseTypeCore == BaseTypeCore && type.Assembly == Assembly;
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Name.GetHashCode() ^ BaseTypeCore.GetHashCode() ^ Assembly.GetHashCode();
        }

        protected override TypeAttributes GetAttributeFlagsImpl() {
            return TypeAttributes.Interface | TypeAttributes.Public;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
                                                              CallingConventions callConvention, Type[] types,
                                                              ParameterModifier[] modifiers) {
            return null;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
            return BaseTypeCore.GetConstructors(bindingAttr);
        }

        public override Type GetElementType() {
            return BaseTypeCore.GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
            return BaseTypeCore.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
            return BaseTypeCore.GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
            return BaseTypeCore.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
            return BaseTypeCore.GetFields(bindingAttr);
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
            return BaseTypeCore.GetMembers(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
                                                    CallingConventions callConvention, Type[] types,
                                                    ParameterModifier[] modifiers) {
            return null;
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
            return BaseTypeCore.GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr) {
            return null;
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
            return EmptyTypes;
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
                                                        Type returnType, Type[] types, ParameterModifier[] modifiers) {
            return GetProperties(bindingAttr).FirstOrDefault(property => property.Name == name);
        }

        protected override bool HasElementTypeImpl() {
            return false;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
                                            object[] args, ParameterModifier[] modifiers, CultureInfo culture,
                                            string[] namedParameters) {
            return null;
        }

        protected override bool IsArrayImpl() {
            return false;
        }

        protected override bool IsByRefImpl() {
            return false;
        }

        protected override bool IsCOMObjectImpl() {
            return false;
        }

        protected override bool IsPointerImpl() {
            return false;
        }

        protected override bool IsPrimitiveImpl() {
            return false;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return attributesCore.Cast<object>().Where(attributeType.IsInstanceOfType).ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return attributesCore.OfType<object>().ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return false;
        }

        public override Type GetInterface(string name, bool ignoreCase) {
            return BaseTypeCore.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces() {
            return BaseTypeCore.GetInterfaces();
        }
    }
}