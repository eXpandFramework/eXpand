using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Navigation
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new XpandNavigationItemAttribute("Navigation/Detail View of Persistent object with records", "Customer_DetailView");
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderDetailViewNavigation, "1=1", "1=1", Captions.ViewMessageDetailViewNavigation, Position.Bottom) { View = "Customer_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderDetailViewNavigation, "1=1", "1=1", Captions.HeaderDetailViewNavigation, Position.Top) { View = "Customer_DetailView" };
        }
    }
}
