using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base {
    public class AllTypesLocalizedClassInfoTypeConverter : ReferenceConverter {
        private static readonly Type[] Types;

        static AllTypesLocalizedClassInfoTypeConverter() {
            Types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => {
                Type[] types=Type.EmptyTypes;
                try {
                    types = assembly.GetTypes();
                }
                catch (Exception e) {
                    Tracing.Tracer.LogError(e);
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
                var type = val as Type;
                if (type != null) {
                    var boModel = CaptionHelper.ApplicationModel?.BOModel;
                    if(boModel == null) {
                        return GetTypeName(type: type);
                    }
                    var modelClass = boModel[type.FullName];
                    return modelClass != null ? modelClass.Caption : GetTypeName(type);
                }
                return CaptionHelper.NoneValue;
            }

            return base.ConvertTo(context, culture, val, destType);
        }

        private  string GetTypeName(Type type) {
            return type.IsNullableType()
                ? type.GenericTypeArguments.Any() ? $"{typeof(Nullable<>)}[{type.GenericTypeArguments[0].Name}" :
                $"{typeof(Nullable<>)}"
                : type.Name;
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