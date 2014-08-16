using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.HideToolBar.ListView {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.ViewMessageHideListViewToolBar, Position.Bottom) { ViewType = ViewType.ListView, View = "HideListViewToolBar_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.HeaderHideListViewToolBar, Position.Top) { ViewType = ViewType.ListView, View = "HideListViewToolBar_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "HideListViewToolBar_ListView");
            yield return new XpandNavigationItemAttribute("Hide Tool Bar/ListView", "HideListViewToolBar_ListView");
            yield return new DisplayFeatureModelAttribute("HideListViewToolBar_ListView");
        }
    }
}
