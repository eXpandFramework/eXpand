using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core
{
    public static class DictionaryNodeExtensions
    {
        public static bool IsView(this DictionaryNode node)
        {
            return new ApplicationNodeWrapper(node.Dictionary).Views.FindViewById(node.GetAttributeValue("ID"))!=null;
        }
    }
}