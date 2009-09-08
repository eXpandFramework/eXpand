using System;
using System.IO;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using eXpand.Utils.GeneralDataStructures;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters{
    public class DictionaryValueConverter:ValueConverter{
        public override Type StorageType{
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value){
            var node = value as Dictionary;
            if (node != null){
                var dictionary = new SerializableDictionary<string, string>{{"Schema", node.Schema.RootNode.ToXml()}};
                foreach (var aspect in node.Aspects){
                    dictionary.Add(aspect,new DictionaryXmlWriter().GetAspectXml(aspect, node.RootNode)); 
                }
                
                var stringWriter = new StringWriter();
                var writer = new XmlTextWriter(stringWriter);
                dictionary.WriteXml(writer);
                var s = stringWriter.GetStringBuilder().ToString();
                return s;
            }
            return null;
        }

        public override object ConvertFromStorageType(object value){
            if (!(string.IsNullOrEmpty(value as string)))
            {
                var settings = new XmlReaderSettings{ConformanceLevel = ConformanceLevel.Auto};
                var reader = XmlReader.Create(new StringReader((string) value), settings);
                var serializableDictionary = new SerializableDictionary<string, string>();
                serializableDictionary.ReadXml(reader);
                var rootNode = new DictionaryXmlReader().ReadFromString(serializableDictionary[DictionaryAttribute.DefaultLanguage]);
                var schema = new Schema(new DictionaryXmlReader().ReadFromString(serializableDictionary["Schema"].Replace(":","")));
                var dictionary = new Dictionary(rootNode, schema);
                foreach (var valuePair in serializableDictionary.Where(pair => pair.Key!=DictionaryAttribute.DefaultLanguage&&pair.Key!="Schema")){
                    var dictionaryNode = new DictionaryXmlReader().ReadFromString(valuePair.Value);
                    dictionary.AddAspect(valuePair.Key, dictionaryNode);    
                }
                return dictionary;
            }
            return null;
        }
    }

}