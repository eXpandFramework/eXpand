using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Navigation.TabStopForReadOnly
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderTabStopForReadOnly, "1=1", "1=1",
                Captions.ViewMessageTabStopForReadOnly, Position.Bottom){View = "TabStopForReadOnly_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderTabStopForReadOnly, "1=1", "1=1",
                Captions.HeaderTabStopForReadOnly, Position.Top){View = "TabStopForReadOnly_DetailView"};
            yield return new CloneViewAttribute(CloneViewType.DetailView, "TabStopForReadOnly_DetailView");
            yield return new XpandNavigationItemAttribute("Navigation/Tab Stop For ReadOnly", "TabStopForReadOnly_DetailView");
            new DisplayFeatureModelAttribute("TabStopForReadOnly_DetailView");
        }
    }
}
