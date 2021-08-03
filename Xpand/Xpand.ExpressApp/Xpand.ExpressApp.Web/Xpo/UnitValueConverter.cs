using System;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp.Web.Xpo {
    public class UnitValueConverter : ValueConverter {
        public override Type StorageType => typeof(string);

        public override object ConvertToStorageType(object value) {
            Unit unit = value is Unit unit1 ? unit1 : Unit.Empty;
            var convertToStorageType = new XmlSerializer(typeof(Unit));
            var memoryStream = new MemoryStream();
            convertToStorageType.Serialize(memoryStream, unit);
            return new StreamReader(memoryStream).ReadToEnd();
        }

        public override object ConvertFromStorageType(object value) {
            if (value == null)
                return null;
            var xmlSerializer = new XmlSerializer(typeof(Unit));
            return xmlSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(value.ToString())));
        }
    }
}