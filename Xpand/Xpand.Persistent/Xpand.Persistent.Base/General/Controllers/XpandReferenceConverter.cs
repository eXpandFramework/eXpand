using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers {
    public abstract class XpandReferenceConverter : ReferenceConverter {
        protected XpandReferenceConverter()
            : base(typeof(Type)) {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType) {
            return ((Type)(value)).FullName;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            Debug.Assert(value != null, "value != null");
            return ReflectionHelper.FindType(value.ToString());
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(GetTypeInfo());
            var typeInfos = GetTypeInfos(typeInfo);
            return new StandardValuesCollection(typeInfos.ToArray());
        }

        protected virtual IEnumerable<Type> GetTypeInfos(ITypeInfo typeInfo){
            return ReflectionHelper.FindTypeDescendants(typeInfo).Select(info => info.Type);
        }

        protected abstract Type GetTypeInfo();
    }
}