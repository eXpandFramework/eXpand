using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp {
    public abstract class XpandReferenceConverter : ReferenceConverter {
        protected XpandReferenceConverter()
            : base(typeof(Type))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof (string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType) {
            return ((Type) (value)).FullName;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return ReflectionHelper.FindType(value.ToString());
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(GetTypeInfo());
            List<Type> typeInfos = ReflectionHelper.FindTypeDescendants(typeInfo).Select(info => info.Type).ToList();
            return new StandardValuesCollection(typeInfos);
        }

        protected abstract Type GetTypeInfo();
    }
}