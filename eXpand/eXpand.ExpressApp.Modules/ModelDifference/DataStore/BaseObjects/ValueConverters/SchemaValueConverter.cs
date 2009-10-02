using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters{
    public class SchemaValueConverter:ValueConverter{
        public override Type StorageType{
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value){
            var schema = value as Schema;
            if (schema != null) return schema.RootNode.ToXml();
            return null;
        }

        public override object ConvertFromStorageType(object value){
            if (value != null) return new Schema(new DictionaryXmlReader().ReadFromString(((string)value).Replace(":","")));
            return null;
        }
    }
}