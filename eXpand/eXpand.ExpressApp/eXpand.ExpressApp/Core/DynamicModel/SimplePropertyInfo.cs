using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace eXpand.ExpressApp.Core.DynamicModel {
    public sealed class SimplePropertyInfo : PropertyInfo {
        readonly List<object> attributesCore = new List<object>();
        readonly bool canReadCore;
        readonly bool canWriteCore;
        readonly Type declaringTypeCore;
        readonly string nameCore;
        readonly Type propertyTypeCore;

        public SimplePropertyInfo(PropertyInfo sourcePropertyInfo)
            : this(
                sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType, sourcePropertyInfo.DeclaringType,
                sourcePropertyInfo.CanRead, sourcePropertyInfo.CanWrite) {
        }

        public SimplePropertyInfo(string name, Type propertyType, Type declaringType, bool canRead, bool canWrite) {
            nameCore = name;
            propertyTypeCore = propertyType;
            declaringTypeCore = declaringType;
            canReadCore = canRead;
            canWriteCore = canWrite;
        }

        public override string Name {
            get { return nameCore; }
        }

        public override Type PropertyType {
            get { return propertyTypeCore; }
        }

        public override Type DeclaringType {
            get { return declaringTypeCore; }
        }

        public override bool CanRead {
            get { return canReadCore; }
        }

        public override bool CanWrite {
            get { return canWriteCore; }
        }

        public override PropertyAttributes Attributes {
            get { throw new NotImplementedException(); }
        }

        public override Type ReflectedType {
            get { throw new NotImplementedException(); }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic) {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic) {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters() {
            return new ParameterInfo[0];
        }

        public override MethodInfo GetSetMethod(bool nonPublic) {
            return null;
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index,
                                        CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index,
                                      CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return attributesCore.Where(attr => attributeType.IsAssignableFrom(attr.GetType())).ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return attributesCore.ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public object AddAttribute(Attribute attribute) {
            if (attribute != null)
                attributesCore.Add(attribute);
            return attribute;
        }
    }
}