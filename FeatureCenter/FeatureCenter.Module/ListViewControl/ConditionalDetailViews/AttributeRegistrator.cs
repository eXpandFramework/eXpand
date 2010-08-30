using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ListViewControl.ConditionalDetailViews
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public const string ConditionalDetailViews_ListView = "ConditionalDetailViews_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalDetailViews, "1=1", "1=1", Captions.ViewMessageConditionalDetailViews, Position.Bottom){ViewType = ViewType.ListView, View = ConditionalDetailViews_ListView};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalDetailViews, "1=1", "1=1", Captions.HeaderConditionalDetailViews, Position.Top) { ViewType = ViewType.ListView, View = ConditionalDetailViews_ListView };
            yield return new NavigationItemAttribute(Captions.ListViewCotrol + "Conditional DetailViews", ConditionalDetailViews_ListView);
            yield return new CloneViewAttribute(CloneViewType.ListView, ConditionalDetailViews_ListView);
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ConditionalDetailViews_DetailView");
            new DisplayFeatureModelAttribute(ConditionalDetailViews_ListView);
        }
    }
}
