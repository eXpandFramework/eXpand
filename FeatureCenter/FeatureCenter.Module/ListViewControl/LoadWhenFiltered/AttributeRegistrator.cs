using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ListViewControl.LoadWhenFiltered
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public const string LoadWhenFiltered_ListView = "LoadWhenFiltered_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.ViewMessageLoadWhenFiltered, Position.Bottom){ViewType = ViewType.ListView, View = LoadWhenFiltered_ListView};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.HeaderLoadWhenFiltered, Position.Top) { ViewType = ViewType.ListView, View = LoadWhenFiltered_ListView };
            yield return new XpandNavigationItemAttribute(Captions.ListViewCotrol + "Load When Filtered", LoadWhenFiltered_ListView);
            yield return new CloneViewAttribute(CloneViewType.ListView, LoadWhenFiltered_ListView);
            new DisplayFeatureModelAttribute(LoadWhenFiltered_ListView);
        }
    }
}
