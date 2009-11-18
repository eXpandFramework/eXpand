using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core.DictionaryHelpers {
    public static class DictionaryXmlWriterExtensions {
        public static string GetAspectXML(this DictionaryXmlWriter xmlWriter, string aspect,DictionaryNode dictionaryNode)
        {
            return xmlWriter.GetAspectXml(dictionaryNode.Dictionary.GetAspectIndex(aspect), dictionaryNode);
        }    
    }
}