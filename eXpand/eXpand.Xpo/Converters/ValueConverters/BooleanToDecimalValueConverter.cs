using System;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Converters.ValueConverters
{
    /// <summary>
    /// Summary description for BooleanValueConverter.
    /// </summary>
    public class BooleanToDecimalValueConverter : ValueConverter
    {
        public override Type StorageType
        {
            get { return typeof (decimal); }
        }

        public override object ConvertToStorageType(object value)
        {
            return value == null ? null : (object) Convert.ToDecimal(value);
        }

        public override object ConvertFromStorageType(object value)
        {
            return value == null ? null : (object) Convert.ToBoolean(value);
        }
    }
}