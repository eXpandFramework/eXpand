using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Utils.GeneralDataStructures;

namespace Xpand.ExpressApp.ModelDifference {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            if (CurrentDBVersion > new Version(0, 0, 0, 0) && CurrentDBVersion <= new Version(10, 1, 6)) {
                var differenceObjects = new Dictionary<object, string>();
                using (var reader = ExecuteReader("select [Oid], [Model] from [ModelDifferenceObject] where [Model] is not null", false)) {
                    while (reader.Read()) {
                        differenceObjects.Add(reader[0], reader[1] as string);
                    }
                }

                using (var uow = new UnitOfWork(((XPObjectSpace)ObjectSpace).Session.DataLayer)) {
                    foreach (var differenceObject in differenceObjects) {
                        var modelDifferenceObject = uow.GetObjectByKey<ModelDifferenceObject>(differenceObject.Key);
                        var serializableDictionary = new SerializableDictionary<string, string>();
                        var xmlReader = XmlReader.Create(new StringReader(differenceObject.Value), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto });
                        serializableDictionary.ReadXml(xmlReader);
                        var aspects = serializableDictionary["aspects"].Split(',').ToList();
                        var defaultAspect = serializableDictionary["DefaultAspect"];
                        defaultAspect = GetDefaultAspectFromVersion9(serializableDictionary, aspects, defaultAspect);

                        if (!string.IsNullOrEmpty(defaultAspect))
                            modelDifferenceObject.AspectObjects.Add(new AspectObject(uow) { Name = CaptionHelper.DefaultLanguage, Xml = defaultAspect });

                        foreach (var aspect in aspects.Where(aspect => !string.IsNullOrEmpty(aspect) && !string.IsNullOrEmpty(serializableDictionary[aspect]))) {
                            modelDifferenceObject.AspectObjects.Add(new AspectObject(uow) { Name = aspect, Xml = serializableDictionary[aspect] });
                        }
                    }

                    uow.CommitChanges();
                }
            }
        }

        private string GetDefaultAspectFromVersion9(SerializableDictionary<string, string> serializableDictionary, List<string> aspects, string defaultAspect) {
            if (serializableDictionary.ContainsKey("Schema")) {
                return GetAspectFromXml(aspects, defaultAspect);
            }

            return defaultAspect;
        }

        private string GetAspectFromXml(List<string> aspects, string xml) {
            aspects = aspects.OrderBy(s => s).ToList();

            xml = xml.Replace("&#165;", "¥");
            xml = removeSpaces(xml);
            string defaultAspectValuesWhenNoOtherAspectExist = Regex.Replace(xml, "\":([^\"\xA5]*)\"", "\"$1\"");
            string removedAspectsWithNoDefaultAspects = defaultAspectValuesWhenNoOtherAspectExist;
            if (!string.IsNullOrEmpty(aspects[0])) {
                string defaultAspectWhenOtherAspectExists = Regex.Replace(defaultAspectValuesWhenNoOtherAspectExist, @""":([^""\xA5]*)\xA5" + aspects[0] + @":([^""]*)""", "\"$1\"");
                removedAspectsWithNoDefaultAspects = aspects.Aggregate(defaultAspectWhenOtherAspectExists, (current, aspect) => removeAttributesWithNoDefaultValue(aspect, current));
            }
            return removedAspectsWithNoDefaultAspects;
        }

        private string removeSpaces(string aspects) {
            return aspects.Replace(" >", ">");
        }

        private string removeAttributesWithNoDefaultValue(string aspect, string value) {
            return Regex.Replace(value, "( [^=\"]*=\"" + aspect + ":([^\"]*)\")", "");
        }
    }
}
