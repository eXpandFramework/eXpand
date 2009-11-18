using System;
using System.IO;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Utils.GeneralDataStructures;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters{
    public class DictionaryValueConverter:ValueConverter{
        public override Type StorageType{
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value){
            var dictionary = value as Dictionary;
            if (dictionary != null){
                string fullXml = new DictionaryXmlWriterEx().GetFullXml(dictionary.RootNode);
                var serializableDictionary = new SerializableDictionary<string, string>{
                                                                               {"Schema", dictionary.Schema.RootNode.ToXml()},
                                                                               {"DefaultAspect", fullXml}
                                                                           };
                serializableDictionary["aspects"] = "";
                foreach (var aspect in dictionary.Aspects.Where(s1 => s1!=DictionaryAttribute.DefaultLanguage)){
                    serializableDictionary["aspects"] += aspect + ",";
                    serializableDictionary[aspect] = new DictionaryXmlWriter().GetAspectXml(dictionary.GetAspectIndex(aspect), dictionary.RootNode);
                }
                serializableDictionary["aspects"] = serializableDictionary["aspects"].TrimEnd(',');
                
                var stringWriter = new StringWriter();
                var writer = new XmlTextWriter(stringWriter);
                serializableDictionary.WriteXml(writer);
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
                var schema = new Schema(new DictionaryXmlReader().ReadFromString(serializableDictionary["Schema"].Replace(":","")));
                var helper = new DictionaryHelper();
                var aspects = serializableDictionary["aspects"].Split(',').ToList();

                string aspectFromXml = helper.GetAspectFromXml(aspects, serializableDictionary["DefaultAspect"]);
                var dictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(aspectFromXml), schema);
                foreach (var aspectValue in aspects.Where(s => !string.IsNullOrEmpty(s))){
                    string xml = serializableDictionary[aspectValue];
                    if (!(string.IsNullOrEmpty(xml)))
                        dictionary.AddAspect(aspectValue, new DictionaryXmlReader().ReadFromString(xml));
                }

                return dictionary;

            }
            return null;
        }
    }
//    public class DictionaryValueConverter1:ValueConverter{
//        public override Type StorageType{
//            get { return typeof(string); }
//        }
//
//        public override object ConvertToStorageType(object value){
//            var dictionary = value as Dictionary;
//            if (dictionary != null){
//                string fullXml = new DictionaryXmlWriterEx().GetFullXml(dictionary.RootNode);
//                var serializableDictionary = new SerializableDictionary<string, string>{
//                                                                               {"Schema", dictionary.Schema.RootNode.ToXml()},
//                                                                               {"DefaultAspect", fullXml}
//                                                                           };
//                serializableDictionary["aspects"] = "";
//                foreach (var aspect in dictionary.Aspects.Where(s1 => s1!=DictionaryAttribute.DefaultLanguage)){
//                    serializableDictionary["aspects"] += aspect + ",";
//                    serializableDictionary[aspect] = new DictionaryXmlWriter().GetAspectXml(aspect, dictionary.RootNode);
//                }
//                serializableDictionary["aspects"] = serializableDictionary["aspects"].TrimEnd(',');
//                
//                var stringWriter = new StringWriter();
//                var writer = new XmlTextWriter(stringWriter);
//                serializableDictionary.WriteXml(writer);
//                var s = stringWriter.GetStringBuilder().ToString();
//                return s;
//            }
//            return null;
//        }
//
//        public override object ConvertFromStorageType(object value){
//            if (!(string.IsNullOrEmpty(value as string)))
//            {
//                var settings = new XmlReaderSettings{ConformanceLevel = ConformanceLevel.Auto};
//                var reader = XmlReader.Create(new StringReader((string) value), settings);
//                var serializableDictionary = new SerializableDictionary<string, string>();
//                serializableDictionary.ReadXml(reader);
//                var schema = new Schema(new DictionaryXmlReader().ReadFromString(serializableDictionary["Schema"].Replace(":","")));
//                var helper = new DictionaryHelper();
//                var aspects = new List<string>();
//                foreach (var aspect in serializableDictionary["aspects"].Split(',')){
//                    aspects.Add(aspect);
//                }
//                string aspectFromXml = helper.GetAspectFromXml(aspects, serializableDictionary["DefaultAspect"]);
//                var dictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(aspectFromXml), schema);
//                foreach (var aspectValue in aspects.Where(s => !string.IsNullOrEmpty(s))){
//                    string xml = serializableDictionary[aspectValue];
//                    if (!(string.IsNullOrEmpty(xml)))
//                        dictionary.AddAspect(aspectValue, new DictionaryXmlReader().ReadFromString(xml));
//                }
//
//                return dictionary;
//
//            }
//            return null;
//        }
//    }
//    public class DictionaryValueConverter1:ValueConverter{
//        public override Type StorageType{
//            get { return typeof(string); }
//        }
//
//        public override object ConvertToStorageType(object value){
//            var dictionary = value as Dictionary;
//            if (dictionary != null){
//                string fullXml = new DictionaryXmlWriterEx().GetFullXml(dictionary.RootNode);
//                var serializableDictionary = new SerializableDictionary<string, string>{
//                                                                               {"Schema", dictionary.Schema.RootNode.ToXml()},
//                                                                               {"DefaultAspect", fullXml}
//                                                                           };
//                serializableDictionary["aspects"] = "";
//                foreach (var aspect in dictionary.Aspects.Where(s1 => s1!=DictionaryAttribute.DefaultLanguage)){
//                    serializableDictionary["aspects"] += aspect + ",";
//                    serializableDictionary[aspect] = new DictionaryXmlWriter().GetAspectXml(aspect, dictionary.RootNode);
//                }
//                serializableDictionary["aspects"] = serializableDictionary["aspects"].TrimEnd(',');
//                
//                var stringWriter = new StringWriter();
//                var writer = new XmlTextWriter(stringWriter);
//                serializableDictionary.WriteXml(writer);
//                var s = stringWriter.GetStringBuilder().ToString();
//                return s;
//            }
//            return null;
//        }
//
//        public override object ConvertFromStorageType(object value){
//            if (!(string.IsNullOrEmpty(value as string)))
//            {
//                var settings = new XmlReaderSettings{ConformanceLevel = ConformanceLevel.Auto};
//                var reader = XmlReader.Create(new StringReader((string) value), settings);
//                var serializableDictionary = new SerializableDictionary<string, string>();
//                serializableDictionary.ReadXml(reader);
//                var schema = new Schema(new DictionaryXmlReader().ReadFromString(serializableDictionary["Schema"].Replace(":","")));
//                var helper = new DictionaryHelper();
//                var aspects = new List<string>();
//                foreach (var aspect in serializableDictionary["aspects"].Split(',')){
//                    aspects.Add(aspect);
//                }
//                string aspectFromXml = helper.GetAspectFromXml(aspects, serializableDictionary["DefaultAspect"]);
//                var dictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(aspectFromXml), schema);
//                foreach (var aspectValue in aspects.Where(s => !string.IsNullOrEmpty(s))){
//                    string xml = serializableDictionary[aspectValue];
//                    if (!(string.IsNullOrEmpty(xml)))
//                        dictionary.AddAspect(aspectValue, new DictionaryXmlReader().ReadFromString(xml));
//                }
//
//                return dictionary;
//
//            }
//            return null;
//        }
//    }


}