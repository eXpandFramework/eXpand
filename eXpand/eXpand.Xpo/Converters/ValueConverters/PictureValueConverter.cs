using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Converters.ValueConverters
{
    public class PictureValueConverter : ValueConverter
    {
        public override Type StorageType
        {
            get { return typeof (byte[]); }
        }

        public override object ConvertToStorageType(object value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                MemoryStream m = new MemoryStream();
                ((Image) value).Save(m, ImageFormat.Jpeg);
                return m.GetBuffer();
            }
        }

        public override object ConvertFromStorageType(object value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                MemoryStream m = new MemoryStream((byte[]) value);
                return new Bitmap(m);
            }
        }
    }
}