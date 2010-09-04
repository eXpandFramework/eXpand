using System;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters {
    public class UnitValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof (string); }
        }

        public override object ConvertToStorageType(object value) {
            Unit unit = value is Unit ? (Unit) value : Unit.Empty;
            var convertToStorageType = new XmlSerializer(typeof (Unit));
            var memoryStream = new MemoryStream();
            convertToStorageType.Serialize(memoryStream, unit);
            return new StreamReader(memoryStream).ReadToEnd();
        }

        public override object ConvertFromStorageType(object value) {
            if (value == null)
                return value;
            var xmlSerializer = new XmlSerializer(typeof (Unit));
            return xmlSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(value.ToString())));
        }
    }
}