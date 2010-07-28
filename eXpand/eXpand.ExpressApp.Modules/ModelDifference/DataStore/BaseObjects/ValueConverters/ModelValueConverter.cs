using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Utils.GeneralDataStructures;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters
{
    public class ModelValueConverter : ValueConverter
    {
        public override Type StorageType
        {
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value)
        {
            var model = value as ModelApplicationBase;
            if (model != null)
            {
                var writer = new ModelXmlWriter();
                var serializableDictionary = new SerializableDictionary<string, string>();
                serializableDictionary["aspects"] = string.Empty;
                for (int i = 0; i < model.AspectCount; ++i)
                {
                    var aspect = model.GetAspect(i);
                    if (string.IsNullOrEmpty(aspect) || aspect == CaptionHelper.DefaultLanguage){
                        serializableDictionary["DefaultAspect"] = writer.WriteToString(model, i);
                    }
                    else{
                        serializableDictionary["aspects"] += aspect + ",";
                        serializableDictionary[aspect] = writer.WriteToString(model, i);
                    }
                }

                serializableDictionary["aspects"] = serializableDictionary["aspects"].TrimEnd(',');

                var stringWriter = new StringWriter();
                serializableDictionary.WriteXml(new XmlTextWriter(stringWriter));
                return stringWriter.GetStringBuilder().ToString();
            }

            return null;
        }



        public override object ConvertFromStorageType(object value)
        {
            var masterModel = ModelDifferenceModule.MasterModel;
            var layer = masterModel.CreatorInstance.CreateModelApplication();
            
            if (!(string.IsNullOrEmpty(value as string)))
            {
                masterModel.AddLayer(layer);
                var serializableDictionary = new SerializableDictionary<string, string>();
                var xmlReader = XmlReader.Create(new StringReader((string)value), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto });
                serializableDictionary.ReadXml(xmlReader);
                var aspects = serializableDictionary["aspects"].Split(',').ToList();
                var defaultAspect = serializableDictionary["DefaultAspect"];
                defaultAspect = GetDefaultAspectFromVersion9(serializableDictionary, aspects, defaultAspect);

                if (!string.IsNullOrEmpty(defaultAspect))
                    new ModelXmlReader().ReadFromString(layer, String.Empty, defaultAspect);

                foreach (var aspect in aspects.Where(aspect => !string.IsNullOrEmpty(aspect) && !string.IsNullOrEmpty(serializableDictionary[aspect]))){
                    new ModelXmlReader().ReadFromString(layer, aspect, serializableDictionary[aspect]);
                }

            }

            return layer;
        }

        string GetDefaultAspectFromVersion9(SerializableDictionary<string, string> serializableDictionary, List<string> aspects, string defaultAspect) {
            if (serializableDictionary.ContainsKey("Schema")){
                var helper = new DictionaryHelper();
                defaultAspect = helper.GetAspectFromXml(aspects, defaultAspect);
            }
            return defaultAspect;
        }

        
    }
}