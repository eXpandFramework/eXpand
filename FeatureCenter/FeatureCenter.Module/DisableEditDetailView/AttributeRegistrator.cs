using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.DisableEditDetailView
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
                Captions.ViewMessageDisableEditDetailView, Position.Bottom){View = "DisableEditDetailView_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
                Captions.HeaderDisableEditDetailView, Position.Top){View = "DisableEditDetailView_DetailView"};
            yield return new CloneViewAttribute(CloneViewType.DetailView, "DisableEditDetailView_DetailView");
            yield return new NavigationItemAttribute(Captions.DetailViewCotrol + "Disable Edit Detail View", "DisableEditDetailView_DetailView");
            new DisplayFeatureModelAttribute("DisableEditDetailView_DetailView");
        }
    }
}
