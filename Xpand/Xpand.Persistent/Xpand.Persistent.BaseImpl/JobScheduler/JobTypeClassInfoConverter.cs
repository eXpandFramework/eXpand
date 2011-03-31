using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class JobTypeClassInfoConverter : ClassInfoTypeConverter {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo("Quartz.IJob");
            var typeInfos = ReflectionHelper.FindTypeDescendants(typeInfo).Where(info => !info.IsInterface && !info.IsAbstract && !(info.Type.FullName+"").StartsWith("Quartz")).Select(info => info.Type).ToList();
            return new StandardValuesCollection(typeInfos);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object val, System.Type destType) {
            if (destType==typeof(string)) {
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo((Type) val);
                var displayNameAttribute = typeInfo.FindAttribute<DisplayNameAttribute>();
                if (displayNameAttribute!=null)
                    return  displayNameAttribute.DisplayName;
            }
            return base.ConvertTo(context, culture, val, destType);
        }
    }
}