using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.Core
{
    public static class ModelXmlReaderExtensions
    {
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom) {
            ReadFromModel(modelXmlReader, modelNode, readFrom, null);
        }
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom, Func<string, bool> aspectNamePredicate)
        {
            var aspectNames = GetAspectNames(readFrom, aspectNamePredicate);
            for (int i = 0; i < aspectNames.ToList().Count; i++) {
                string aspect = readFrom.GetAspect(i);
                string xml = new ModelXmlWriter().WriteToString(readFrom, i);
                if (!(string.IsNullOrEmpty(xml)))
                    new ModelXmlReader().ReadFromString(modelNode, aspect, xml);
            }
        }

        static IEnumerable<string> GetAspectNames(ModelApplicationBase readFrom, Func<string, bool> aspectNamePredicate) {
            return aspectNamePredicate!= null ? readFrom.GetAspectNames().Where(aspectNamePredicate) : readFrom.GetAspectNames();
        }
    }
}
