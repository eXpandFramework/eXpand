using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class ViewVisibilityCalculator:AttributeVisibilityCalculator
    {
        private readonly ExpressionParamsParser helper;
        public ViewVisibilityCalculator(string parameter) : base(parameter)
        {
            helper = new ExpressionParamsParser(parameter);
        }


        public override bool IsInvisible(DictionaryNode node, string attributeName)
        {
            var attributeValueByPath = helper.GetAttributeValueByPath(node, helper.GetParamValue("ID", "@ID"));
            if (!string.IsNullOrEmpty(attributeValueByPath))
            {
                var wrapper = new ApplicationNodeWrapper(node.Dictionary).Views.FindViewById(attributeValueByPath);
                if ((helper.GetParamValue("ViewType") == ViewType.DetailView.ToString() && wrapper is DetailViewInfoNodeWrapper) ||
                    (helper.GetParamValue("ViewType") == ViewType.ListView.ToString() && wrapper is ListViewInfoNodeWrapper))
                    return true;
                if (helper.GetParamValue("ViewType") == ViewType.Any.ToString() && wrapper != null)
                    return true;
            }
            
            return false;
        }
    }
}