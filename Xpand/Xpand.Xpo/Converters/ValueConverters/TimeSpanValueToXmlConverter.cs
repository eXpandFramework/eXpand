using System;
using System.Xml;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters{
    public class TimeSpanValueToXmlConverter : ValueConverter {
        public override object ConvertFromStorageType(object value) {
            return value == null ? TimeSpan.Zero : XmlConvert.ToTimeSpan((String)value);
        }
        public override object ConvertToStorageType(object value) {
            return value == null ? null : XmlConvert.ToString((TimeSpan)value);
        }
        public override Type StorageType { get { return typeof(String); } }
    }
}