using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.HideToolBar.DetailView
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.ViewMessageHideDetailViewToolBar, Position.Bottom){ViewType = ViewType.DetailView, View = "HideDetailViewToolBar_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.HeaderHideDetailViewToolBar, Position.Top) { ViewType = ViewType.DetailView, View = "HideDetailViewToolBar_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "HideDetailViewToolBar_DetailView");
            yield return new XpandNavigationItemAttribute("Hide Tool Bar/DetailView", "HideDetailViewToolBar_DetailView");
            yield return new DisplayFeatureModelAttribute("HideDetailViewToolBar_DetailView");
        }
    }
}
