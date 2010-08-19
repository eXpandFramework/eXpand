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
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderViewVariants, "1=1", "1=1", Captions.ViewMessageViewVariants, Position.Bottom) { ViewType = ViewType.ListView, View = "RuntimeFieldsFromModel_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderViewVariants, "1=1", "1=1",
                Captions.HeaderViewVariants, Position.Top) { View = "ViewVariants_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "ViewVariants_ListView");
            yield return new NavigationItemAttribute("View Variants", "ViewVariants_ListView");
            yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
        }
    }
}
