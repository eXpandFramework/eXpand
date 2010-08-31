using System;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.Core
{
    public static class ModelXmlReaderExtensions
    {
        public static void ReadFromString(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom) {
            ReadFromString(modelXmlReader, modelNode, readFrom, null);
        }
        public static void ReadFromString(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom, Func<string, bool> aspectNamePredicate)
        {
            var aspectNames = readFrom.GetAspectNames().Where(aspectNamePredicate);
            for (int i = 0; i < aspectNames.ToList().Count; i++) {
                string aspect = readFrom.GetAspect(i);
                string xml = new ModelXmlWriter().WriteToString(readFrom, i);
                if (!(string.IsNullOrEmpty(xml)))
                    new ModelXmlReader().ReadFromString(modelNode, aspect, xml);
            }
        }
    }
}
