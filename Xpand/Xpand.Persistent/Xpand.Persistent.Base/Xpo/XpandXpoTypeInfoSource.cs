using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Xpo.MetaData;
using TypeInfo = DevExpress.ExpressApp.DC.TypeInfo;

namespace Xpand.Persistent.Base.Xpo {
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

        public XpandXpoTypeInfoSource(TypesInfo typesInfo, params Type[] types) : base(typesInfo, types) {
        }

        public XpandXpoTypeInfoSource(TypesInfo typesInfo, XPDictionary dictionary) : base(typesInfo, dictionary) {
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
            foreach (DCPropertyInfo dcPropertyInfo in dcPropertyInfos) {
                handler(dcPropertyInfo, dcPropertyInfo.Name);
            }
        }

        IEnumerable<DCPropertyInfo> DCPropertyInfos(XPClassInfo classInfo) {
            if (classInfo != null) {
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
        readonly XpandCustomMemberInfo _xpandCustomMemberInfo;
        readonly XPClassInfo _declaringTypeCore;

        public DCPropertyInfo(string name, Type propertyType, XPClassInfo declaringType, bool canRead, bool canWrite, XpandCustomMemberInfo xpandCustomMemberInfo) {
            Name = name;
            PropertyType = propertyType;
            _declaringTypeCore = declaringType;
            CanRead = canRead;
            CanWrite = canWrite;
            _xpandCustomMemberInfo = xpandCustomMemberInfo;
        }


        public override string Name { get; }

        public override Type PropertyType { get; }

        public override Type DeclaringType => _declaringTypeCore.ClassType;

        public override bool CanRead { get; }

        public override bool CanWrite { get; }

        public override PropertyAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public override Type ReflectedType
        {
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

    }
}