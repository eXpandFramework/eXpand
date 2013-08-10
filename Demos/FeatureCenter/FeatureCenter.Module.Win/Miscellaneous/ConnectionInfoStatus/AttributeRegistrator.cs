using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.Miscellaneous.ConnectionInfoStatus {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Module.Captions.HeaderConnectionInfoStatus, "1=1", "1=1", Module.Captions.HeaderConnectionInfoStatus, Position.Top) { View = "ConnectionInfoStatus_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Module.Captions.HeaderConnectionInfoStatus, "1=1", "1=1",
                Module.Captions.ViewMessageConnectionInfoStatus, Position.Bottom) { View = "ConnectionInfoStatus_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ConnectionInfoStatus_DetailView");
            yield return new XpandNavigationItemAttribute(Module.Captions.Miscellaneous + "Connection Info Status", "ConnectionInfoStatus_DetailView");
            yield return new DisplayFeatureModelAttribute("ConnectionInfoStatus_DetailView");
        }
    }
}
