using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class RefNodeStringPropertyProvider : RefNodePropertyProvider
    {
        public RefNodeStringPropertyProvider(string parameter)
            : base(parameter) { }

        protected override ReadOnlyDictionaryNodeCollection GetNodesCollectionInternal(DictionaryNode node, string attributeName)
        {
            var allNodes = base.GetNodesCollectionInternal(node, attributeName);
            var result = new DictionaryNodeCollection();
            var nonStringProperties = allNodes.Where(n => n.GetAttributeValue("Type").Equals(typeof(string).FullName));
            
            foreach (var stringProperty in nonStringProperties)
            {
                result.Add(stringProperty);
            }

            return result;
        }
    }
}
