using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.ControllingSearch.ListView {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.ViewMessageControllingListViewSearch, Position.Bottom) { ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.HeaderControllingListViewSearch, Position.Top) { ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "ControllingSearchCustomer_ListView");
            yield return new XpandNavigationItemAttribute("Controlling Search/ListView Search", "ControllingSearchCustomer_ListView");
            yield return new DisplayFeatureModelAttribute("ControllingSearchCustomer_ListView", "ControllingListViewSearch");
        }
    }
}
