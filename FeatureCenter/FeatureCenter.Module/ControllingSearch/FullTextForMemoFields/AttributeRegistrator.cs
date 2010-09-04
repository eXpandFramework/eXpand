using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ControllingSearch.FullTextForMemoFields
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.ViewMessageDisableFullTextForMemoFields, Position.Bottom){ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.HeaderDisableFullTextForMemoFields, Position.Top) { ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "DisableFullTextForMemoFields_ListView");
            yield return new XpandNavigationItemAttribute("Controlling Search/Disable Full Text For Memo Fields", "DisableFullTextForMemoFields_ListView");
            yield return new DisplayFeatureModelAttribute("DisableFullTextForMemoFields_ListView");
        }
    }
}
