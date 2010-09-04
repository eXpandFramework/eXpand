using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.HideToolBar.NestedListView
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(Customer))
            {
                yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderHideListViewToolBar+"Nested", "1=1", "1=1", Captions.ViewMessageHideListViewToolBarNested, Position.Bottom){View = "HideNestedListViewToolBar_DetailView"};
                yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderHideListViewToolBar + "Nested", "1=1", "1=1", Captions.HeaderHideListViewToolBar, Position.Top) { View = "HideNestedListViewToolBar_DetailView" };
                yield return new CloneViewAttribute(CloneViewType.DetailView, "HideNestedListViewToolBar_DetailView");
                yield return new XpandNavigationItemAttribute("Hide Tool Bar/NestedListView", "HideNestedListViewToolBar_DetailView");
                yield return new DisplayFeatureModelAttribute("HideNestedListViewToolBar_DetailView");
            }
            if (typesInfo.Type==typeof(Order)) {
                new CloneViewAttribute(CloneViewType.ListView, "HideNestedListToolBarView_ListView");
            }
        }
    }
}
