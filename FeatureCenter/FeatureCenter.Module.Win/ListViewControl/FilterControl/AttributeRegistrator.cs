using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ListViewControl.FilterControl
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderFilterControl, "1=1", "1=1",
                Captions.ViewMessageFilterControl, Position.Bottom){View = "FilterControl_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderFilterControl, "1=1", "1=1",
                Captions.HeaderFilterControl, Position.Top){View = "FilterControl_ListView"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "FilterControl_ListView");
            yield return new NavigationItemAttribute(Module.Captions.ListViewCotrol +"Filter Control", "FilterControl_ListView");
            yield return new DisplayFeatureModelAttribute("FilterControl_ListView");
        }
    }
}
