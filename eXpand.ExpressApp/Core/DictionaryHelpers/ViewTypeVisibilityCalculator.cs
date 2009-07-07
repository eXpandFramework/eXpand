using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class ViewTypeVisibilityCalculator:AttributeVisibilityCalculator
    {
        public ViewTypeVisibilityCalculator(string parameter) : base(parameter)
        {
        }


        public override bool IsInvisible(DictionaryNode node, string attributeName)
        {
            return node.IsView();
        }
    }
}