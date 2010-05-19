using System;
using System.IO;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Utils.GeneralDataStructures;
using System.Linq;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters{
    public class ModelValueConverter:ValueConverter{
        public override Type StorageType{
            get { return typeof(string); }
        }

        public override object ConvertToStorageType(object value){
            var model = value as ModelApplicationBase;
            if (model != null){
                var writer = new ModelXmlWriter();
                var serializableDictionary = new SerializableDictionary<string, string>();
                serializableDictionary["aspects"] = string.Empty;
                for (int i = 0; i < model.AspectCount; ++i)
                {
                    var aspect = model.GetAspect(i);
                    if (string.IsNullOrEmpty(aspect) || aspect == CaptionHelper.DefaultLanguage)
                    {
                        aspect = "DefaultAspect";
                    }
                    serializableDictionary["aspects"] += aspect + ",";
                    serializableDictionary[aspect] = writer.WriteToString(model, i).TrimEnd(',');
                }
                
                serializableDictionary["aspects"] = serializableDictionary["aspects"].TrimEnd(',');
                
                var stringWriter = new StringWriter();
                serializableDictionary.WriteXml(new XmlTextWriter(stringWriter));
                return stringWriter.GetStringBuilder().ToString();
            }

            return null;
        }

        public override object ConvertFromStorageType(object value){
            if (!(string.IsNullOrEmpty(value as string)))
            {
                var model = ((ModelNode)ModelDifferenceModule.XafApplication.Model).CreatorInstance.CreateModelApplication();
                var modelReader = new ModelXmlReader();
                var xmlReader = XmlReader.Create(new StringReader((string)value), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto });
                var serializableDictionary = new SerializableDictionary<string, string>();
                serializableDictionary.ReadXml(xmlReader);

                var aspects = serializableDictionary["aspects"].Split(',').ToList();
                foreach (var aspectValue in aspects.Where(aspect => !string.IsNullOrEmpty(aspect))){
                    modelReader.ReadFromString(model, aspectValue == "DefaultAspect" ? string.Empty : aspectValue, serializableDictionary[aspectValue]);
                }

                return model;
            }

            return null;
        }
    }
}