using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.DetailViewControl.ActionButtonViewItem
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
                Captions.ViewMessageActionButtonViewItem, Position.Bottom){ View = "ActionButtonViewItem_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
                Captions.HeaderActionButtonViewItem, Position.Top){View = "ActionButtonViewItem_DetailView"};
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ActionButtonViewItem_DetailView");
            yield return new NavigationItemAttribute(Captions.DetailViewCotrol + "Action Button View Item", "ActionButtonViewItem_DetailView");
            new DisplayFeatureModelAttribute("ActionButtonViewItem_DetailView");
        }
    }
}
