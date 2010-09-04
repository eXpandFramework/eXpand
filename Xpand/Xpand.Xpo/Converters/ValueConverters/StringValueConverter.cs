using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters
{
    /// <summary>
    /// Summary description for Int32ValueConverter.
    /// </summary>
    public class StringValueConverter : ValueConverter
    {
        public override Type StorageType
        {
            get { return typeof (string); }
        }

        public override object ConvertToStorageType(object value)
        {
            return value == null ? null : value.ToString();
        }

        public override object ConvertFromStorageType(object value)
        {
            return value == null ? null : value.ToString();
        }
    }
}