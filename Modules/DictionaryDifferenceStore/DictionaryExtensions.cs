using DevExpress.ExpressApp;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;

namespace eXpand.ExpressApp.DictionaryDifferenceStore
{
    internal static class DictionaryExtensions
    {
        public static void Combine(this Dictionary dictionary,XpoModelDictionaryDifferenceStore selectedObject,
                                   XpoModelDictionaryDifferenceStore activeStore)
        {
            DictionaryNode activeNode = new DictionaryXmlReader().ReadFromString(activeStore.XmlContent);

            DictionaryNode selectedNode = new DictionaryXmlReader().ReadFromString(selectedObject.XmlContent);
            var selectedDictionary = new Dictionary(selectedNode);
            selectedDictionary.AddAspect(selectedObject.Aspect, activeNode);

            selectedObject.XmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                                        new DictionaryXmlWriter().GetAspectXml(
                                            DictionaryAttribute.DefaultLanguage, selectedDictionary.RootNode);
        }

    }
}