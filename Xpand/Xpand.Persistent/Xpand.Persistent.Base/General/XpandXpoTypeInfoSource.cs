using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.General {
    public sealed class DCPropertyInfo : PropertyInfo {
        readonly List<object> _attributesCore = new List<object>();
        readonly bool _canReadCore;
        readonly bool _canWriteCore;
        readonly XpandCalcMemberInfo _xpandCalcMemberInfo;
        readonly Type _declaringTypeCore;
        readonly string _nameCore;
        readonly Type _propertyTypeCore;

        public DCPropertyInfo(string name, Type propertyType, Type declaringType, bool canRead, bool canWrite, XpandCalcMemberInfo xpandCalcMemberInfo) {
            _nameCore = name;
            _propertyTypeCore = propertyType;
            _declaringTypeCore = declaringType;
            _canReadCore = canRead;
            _canWriteCore = canWrite;
            _xpandCalcMemberInfo = xpandCalcMemberInfo;
            _attributesCore.AddRange(xpandCalcMemberInfo.Attributes);
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
            get { throw new NotImplementedException(); }
        }

        public override string ToString() {
            return _xpandCalcMemberInfo.ToString();
        }
        public override MethodInfo[] GetAccessors(bool nonPublic) {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic) {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters() {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic) {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
            return _xpandCalcMemberInfo.GetValue(obj);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
            _xpandCalcMemberInfo.SetValue(obj, value);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return new object[] { _xpandCalcMemberInfo.FindAttributeInfo(attributeType) };
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return _xpandCalcMemberInfo.Attributes.Cast<object>().ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(Attribute attribute) {
            _xpandCalcMemberInfo.RemoveAttribute(attribute.GetType());
        }

        public void AddAttribute(Attribute attribute) {
            _xpandCalcMemberInfo.AddAttribute(attribute);
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

    public class XpandXpoTypeInfoSource : DevExpress.ExpressApp.DC.Xpo.XpoTypeInfoSource, ITypeInfoSource {
        void ITypeInfoSource.EnumMembers(TypeInfo info, EnumMembersHandler handler) {
            EnumMembers(info, handler);
            Type type = info.Type;
            if (TypeIsKnown(type)) {
                if (type.IsInterface) {
                    EnumDCInterfaceMembers(info, handler);
                }
            }
        }

        public XpandXpoTypeInfoSource(TypesInfo typesInfo)
            : base(typesInfo) {
        }

        void ITypeInfoSource.InitMemberInfo(object member, XafMemberInfo memberInfo) {
            var dcPropertyInfo = member as DCPropertyInfo;
            if (dcPropertyInfo != null) {
                memberInfo.MemberType = ((PropertyInfo)member).PropertyType;
                memberInfo.IsReadOnly = true;
            }
            InitMemberInfo(member, memberInfo);
        }

        private void EnumDCInterfaceMembers(TypeInfo info, EnumMembersHandler handler) {
            var generatedEntityType = GetGeneratedEntityType(info.Type);
            if (generatedEntityType != null) {
                var dcPropertyInfos = DCPropertyInfos(generatedEntityType);
                foreach (var dcPropertyInfo in dcPropertyInfos) {
                    handler(dcPropertyInfo, dcPropertyInfo.Name);
                }
            }
        }

        IEnumerable<DCPropertyInfo> DCPropertyInfos(Type generatedEntityType) {
            var classInfo = XPDictionary.GetClassInfo(generatedEntityType);
            var xpandCalcMemberInfos = classInfo.OwnMembers.OfType<XpandCalcMemberInfo>();
            return xpandCalcMemberInfos.Select(info => DcPropertyInfo(generatedEntityType, info));
        }

        DCPropertyInfo DcPropertyInfo(Type generatedEntityType, XpandCalcMemberInfo info) {
            return new DCPropertyInfo(info.Name, info.MemberType, generatedEntityType, true, false, info);
        }
    }
}