using System;
using System.ComponentModel;
using System.Globalization;

namespace Xpand.Xpo.Converters.TypeConverters
{
    public class Int32TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (DateTime))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is DateTime)
            {
                var dateTime =  value as DateTime?;
                return dateTime;
				
            }

            return base.ConvertFrom(context, culture, value);
        }

    }
}