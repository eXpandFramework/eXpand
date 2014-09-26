using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.HideToolBar.NestedListView {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(Customer)) {
                yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderHideNestedListViewToolBar, "1=1", "1=1", Captions.ViewMessageHideListViewToolBarNested, Position.Bottom) { View = "HideNestedListViewToolBar_DetailView" };
                yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderHideNestedListViewToolBar, "1=1", "1=1", Captions.HeaderHideNestedListViewToolBar, Position.Top) { View = "HideNestedListViewToolBar_DetailView" };
                yield return new CloneViewAttribute(CloneViewType.DetailView, "HideNestedListViewToolBar_DetailView");
                yield return new XpandNavigationItemAttribute("Hide Tool Bar/NestedListView", "HideNestedListViewToolBar_DetailView");
                yield return new DisplayFeatureModelAttribute("HideNestedListViewToolBar_DetailView");
            }
            if (typesInfo.Type == typeof(Order)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "HideNestedListToolBarView_ListView");
            }
        }
    }
}
