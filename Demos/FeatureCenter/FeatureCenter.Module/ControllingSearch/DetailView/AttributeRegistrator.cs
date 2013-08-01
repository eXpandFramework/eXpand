using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.ControllingSearch.DetailView {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.ViewMessageControllingDetailViewSearch, Position.Bottom) { ViewType = ViewType.DetailView, View = "ControllingSearchCustomer_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.HeaderControllingDetailViewSearch, Position.Top) { ViewType = ViewType.DetailView, View = "ControllingSearchCustomer_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ControllingSearchCustomer_DetailView");
            yield return new XpandNavigationItemAttribute("Controlling Search/DetailView Search", "ControllingSearchCustomer_DetailView");
            yield return new DisplayFeatureModelAttribute("ControllingSearchCustomer_DetailView", "ControllingDetailViewSearch");
        }
    }
}
