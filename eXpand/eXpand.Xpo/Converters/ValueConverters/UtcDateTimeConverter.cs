namespace eXpand.Xpo.Converters.ValueConverters
{
    public class UtcDateTimeConverter : DevExpress.Xpo.Metadata.UtcDateTimeConverter
    {
        public override object ConvertToStorageType(object value) {
            object convertToStorageType = base.ConvertToStorageType(value);
            return new SqlDateTimeOverFlowValueConverter().ConvertToStorageType(convertToStorageType);
        }
    }
}
