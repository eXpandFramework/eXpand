namespace Xpand.Xpo.Converters.ValueConverters
{
    public class XpandUtcDateTimeConverter : DevExpress.Xpo.Metadata.UtcDateTimeConverter
    {
        public override object ConvertToStorageType(object value) {
            object convertToStorageType = base.ConvertToStorageType(value);
            return new SqlDateTimeOverFlowValueConverter().ConvertToStorageType(convertToStorageType);
        }
    }
}
