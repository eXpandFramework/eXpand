using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base {
    public class AllTypesLocalizedClassInfoTypeConverter : ReferenceConverter {
        private static readonly Type[] Types;

        static AllTypesLocalizedClassInfoTypeConverter() {
            Types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => {
                Type[] types=Type.EmptyTypes;
                try {
                    types = assembly.GetTypes();
                }
                catch (Exception) {
                    // ignored
                }

                return types;
            }).ToArray();
        }


        public AllTypesLocalizedClassInfoTypeConverter() : base(typeof(Type)) {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(Types);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val,
            Type destType) {
            if (destType == typeof(string)) {
                var classInfo = val as Type;
                if (classInfo != null)
                    return CaptionHelper.GetClassCaption(classInfo.FullName);
                return CaptionHelper.NoneValue;
            }

            return base.ConvertTo(context, culture, val, destType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
            Type result = null;
            if (val != null) {
                var caption = val.ToString();
                foreach (var classInfo in XafTypesInfo.Instance.PersistentTypes)
                    if (classInfo.IsVisible && CaptionHelper.GetClassCaption(classInfo.FullName) == caption) {
                        result = classInfo.Type;
                        break;
                    }
            }

            return result;
        }
    }

    public class XpandLocalizedClassInfoTypeConverter : LocalizedClassInfoTypeConverter {
        public XpandLocalizedClassInfoTypeConverter(){
            AllowAddNonPersistentObjects = true;
        }

        protected override string GetClassCaption(string fullName) {
            var classCaption = base.GetClassCaption(fullName);
            return string.IsNullOrEmpty(classCaption) ? CaptionHelper.ApplicationModel.BOModel[fullName].Id() : classCaption;
        }
    }
}