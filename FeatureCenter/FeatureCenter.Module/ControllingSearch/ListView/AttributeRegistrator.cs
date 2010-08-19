using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ControllingSearch.ListView
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.ViewMessageControllingListViewSearch, Position.Bottom){ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.HeaderControllingListViewSearch, Position.Top) { ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "ControllingSearchCustomer_ListView");
            yield return new NavigationItemAttribute("Controlling Search/ListView Search", "ControllingSearchCustomer_ListView");
            yield return new DisplayFeatureModelAttribute("ControllingSearchCustomer_ListView", "ControllingListViewSearch");
        }
    }
}
