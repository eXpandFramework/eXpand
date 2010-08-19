using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ListViewControl.LoadWhenFiltered
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.ViewMessageLoadWhenFiltered, Position.Bottom){ViewType = ViewType.ListView, View = "LoadWhenFiltered_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.HeaderLoadWhenFiltered, Position.Top) { ViewType = ViewType.ListView, View = "LoadWhenFiltered_ListView" };
            yield return new NavigationItemAttribute(Captions.ListViewCotrol + "Load When Filtered", "LoadWhenFiltered_ListView");
            yield return new CloneViewAttribute(CloneViewType.ListView, "LoadWhenFiltered_ListView");
            new DisplayFeatureModelAttribute("LoadWhenFiltered_ListView");
        }
    }
}
