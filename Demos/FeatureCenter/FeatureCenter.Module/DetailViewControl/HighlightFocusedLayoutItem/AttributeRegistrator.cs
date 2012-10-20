using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.DetailViewControl.HighlightFocusedLayoutItem
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.ViewMessageHighlightFocusedLayoutItem, Position.Bottom){ViewType = ViewType.DetailView, View = "HighlightFocusedLayoutItem_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.HeaderHighlightFocusedLayoutItem, Position.Top) { ViewType = ViewType.DetailView, View = "HighlightFocusedLayoutItem_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "HighlightFocusedLayoutItem_DetailView");
            yield return new XpandNavigationItemAttribute(Captions.DetailViewCotrol + "Highlight Focused Layout Item", "HighlightFocusedLayoutItem_DetailView");
            yield return new DisplayFeatureModelAttribute("HighlightFocusedLayoutItem_DetailView");
        }
    }
}
