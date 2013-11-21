using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General {
    public static class ModelXmlReaderExtensions {
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, IModelNode modelNode,
                                          IModelNode readFrom) {
            ReadFromModel(modelXmlReader, modelNode, readFrom, null);
        }
        public static void ReadFromModel(this ModelXmlReader modelXmlReader, IModelNode modelNode,
                                          IModelNode readFrom, Func<string, bool> aspectNamePredicate) {
            var modelApplication = ((ModelApplicationBase) readFrom.Application);
            for (int i = 0; i < modelApplication.AspectCount; i++) {
                string aspect = modelApplication.GetAspect(i);
                string xml = new ModelXmlWriter().WriteToString(readFrom, i);
                if (!(string.IsNullOrEmpty(xml)))
                    new ModelXmlReader().ReadFromString(modelNode, aspect, xml);
            }
        }
    }
}
