using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Core {
    public static class ModelXmlReaderExtensions {
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom) {
            ReadFromModel(modelXmlReader, modelNode, readFrom, null);
        }
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, ModelApplicationBase modelNode,
                                          ModelApplicationBase readFrom, Func<string, bool> aspectNamePredicate) {
            for (int i = 0; i < readFrom.AspectCount; i++) {
                string aspect = readFrom.GetAspect(i);
                string xml = new ModelXmlWriter().WriteToString(readFrom, i);
                if (!(string.IsNullOrEmpty(xml)))
                    new ModelXmlReader().ReadFromString(modelNode, aspect, xml);
            }
        }

    }
}
