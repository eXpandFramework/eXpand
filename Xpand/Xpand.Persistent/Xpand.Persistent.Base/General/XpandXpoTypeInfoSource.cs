using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.General {
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
                memberInfo.MemberType = dcPropertyInfo.PropertyType;
                memberInfo.IsReadOnly = !dcPropertyInfo.CanWrite;
            }
            InitMemberInfo(member, memberInfo);
        }

        private void EnumDCInterfaceMembers(TypeInfo info, EnumMembersHandler handler) {
            var generatedEntityInfo = info.FindDCXPClassInfo();
            var dcPropertyInfos = DCPropertyInfos(generatedEntityInfo);
            foreach (DCPropertyInfo dcPropertyInfo in dcPropertyInfos){
                handler(dcPropertyInfo, dcPropertyInfo.Name);
            }
        }

        IEnumerable<DCPropertyInfo> DCPropertyInfos(XPClassInfo classInfo) {
            if (classInfo != null){
                var xpandCalcMemberInfos = classInfo.OwnMembers.OfType<XpandCustomMemberInfo>();
                return xpandCalcMemberInfos.Select(info => DcPropertyInfo(classInfo, info));
            }
            return Enumerable.Empty<DCPropertyInfo>();
        }

        DCPropertyInfo DcPropertyInfo(XPClassInfo classInfo, XpandCustomMemberInfo info) {
            return new DCPropertyInfo(info.Name, info.MemberType, classInfo, true, !info.IsReadOnly, info);
        }
    }

    public sealed class DCPropertyInfo : PropertyInfo {
        readonly List<object> _attributesCore = new List<object>();
        readonly bool _canReadCore;
        readonly bool _canWriteCore;
        readonly XpandCustomMemberInfo _xpandCustomMemberInfo;
        readonly XPClassInfo _declaringTypeCore;
        readonly string _nameCore;
        readonly Type _propertyTypeCore;

        public DCPropertyInfo(string name, Type propertyType, XPClassInfo declaringType, bool canRead, bool canWrite, XpandCustomMemberInfo xpandCustomMemberInfo) {
            _nameCore = name;
            _propertyTypeCore = propertyType;
            _declaringTypeCore = declaringType;
            _canReadCore = canRead;
            _canWriteCore = canWrite;
            _xpandCustomMemberInfo = xpandCustomMemberInfo;
            _attributesCore.AddRange(xpandCustomMemberInfo.Attributes);
        }


        public override string Name {
            get { return _nameCore; }
        }

        public override Type PropertyType {
            get { return _propertyTypeCore; }
        }

        public override Type DeclaringType {
            get { return _declaringTypeCore.ClassType; }
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
            return _xpandCustomMemberInfo.ToString();
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
            return _xpandCustomMemberInfo.GetValue(obj);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
            _xpandCustomMemberInfo.SetValue(obj, value);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return new object[] { _xpandCustomMemberInfo.FindAttributeInfo(attributeType) };
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return _xpandCustomMemberInfo.Attributes.Cast<object>().ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(Attribute attribute) {
            _xpandCustomMemberInfo.RemoveAttribute(attribute.GetType());
        }

        public void AddAttribute(Attribute attribute) {
            _xpandCustomMemberInfo.AddAttribute(attribute);
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