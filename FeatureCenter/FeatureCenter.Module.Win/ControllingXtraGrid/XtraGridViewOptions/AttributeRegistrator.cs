using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.XtraGridViewOptions
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderControlXtraGrid, "1=1", "1=1",
                Captions.ViewMessageControlXtraGrid, Position.Bottom){View = "XtraGridViewOptions_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderControlXtraGrid, "1=1", "1=1",
                Captions.HeaderControlXtraGrid, Position.Top){View = "XtraGridViewOptions_ListView"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "XtraGridViewOptions_ListView");
            yield return new XpandNavigationItemAttribute("Controlling XtraGrid/GridView options", "XtraGridViewOptions_ListView");
            new DisplayFeatureModelAttribute("XtraGridViewOptions_ListView");
        }
    }
}
