using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ViewVariants
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(Customer))yield break;
            var list=new List<string> {"ViewVariants_ListView","Hong Kong Customers","London Customers","Paris Customers","New York Customers"};
            for (int i = 0; i < list.Count; i++) {
//                yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderViewVariants+i, "1=1", "1=1", Captions.ViewMessageViewVariants, Position.Bottom) { ViewType = ViewType.ListView, View = list[i] };
//                yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderViewVariants+i, "1=1", "1=1", Captions.HeaderViewVariants, Position.Top) { View = list[i] };
                yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
            }
            yield return new CloneViewAttribute(CloneViewType.ListView, "ViewVariants_ListView");
            yield return new NavigationItemAttribute("View Variants", "ViewVariants_ListView");
            yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
        }
    }
}
