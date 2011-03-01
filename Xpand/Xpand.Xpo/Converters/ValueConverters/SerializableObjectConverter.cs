using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters {
    public class SerializableObjectConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value) {
            if (value == null)
                return null;
            if (!value.GetType().IsSerializable)
                throw new NotSupportedException(String.Format("The given object ({0}) is not serializable.", value));


            string result;

            var stream = new MemoryStream();
            try {
                var binaryFormatter = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Full };
                binaryFormatter.Serialize(stream, value);
                result = Convert.ToBase64String(stream.ToArray());
            } finally {
                stream.Close();
            }

            return result;
        }

        public override object ConvertFromStorageType(object value) {
            object result = null;

            if (value != null) {
                MemoryStream stream = null;
                try {
                    byte[] base64String = Convert.FromBase64String((string)value);
                    stream = new MemoryStream(base64String) { Position = 0 };
                    var binaryFormatter = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Full };
                    result = binaryFormatter.Deserialize(stream);
                } catch (Exception) {
                } finally {
                    if (stream != null) stream.Close();
                }
            } else
                return null;
            return result;
        }
    }
}