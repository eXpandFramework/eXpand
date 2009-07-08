using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core
{
    public static class DictionaryNodeExtensions
    {
        public static bool IsView(this DictionaryNode node)
        {
            DictionaryNode dictionaryNode = node;
            while (dictionaryNode.Parent != null||dictionaryNode.KeyAttribute.Value=="Views")
                dictionaryNode = dictionaryNode.Parent;
            return dictionaryNode.KeyAttribute.Value == "Views";
        }
    }
}