using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ConnectionInfoStatus
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConnectionInfoStatus, "1=1", "1=1", Captions.HeaderConnectionInfoStatus, Position.Top){View = "ConnectionInfoStatus_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConnectionInfoStatus, "1=1", "1=1",
                Captions.ViewMessageConnectionInfoStatus, Position.Bottom){View = "ConnectionInfoStatus_DetailView"};
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ConnectionInfoStatus_DetailView");
            yield return new NavigationItemAttribute(Captions.Miscellaneous + "Connection Info Status", "ConnectionInfoStatus_DetailView");
            yield return new DisplayFeatureModelAttribute("ConnectionInfoStatus_DetailView");
        }
    }
}
