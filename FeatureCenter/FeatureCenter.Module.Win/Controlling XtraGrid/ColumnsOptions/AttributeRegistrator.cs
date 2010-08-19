using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.XtraGrid.XtraGridColumnsOptions
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
                Captions.ViewMessageControlXtraGridColumns, Position.Bottom){View = "XtraGridColumnsOptions_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
                Captions.HeaderControlXtraGridColumns, Position.Top){View = "XtraGridColumnsOptions_ListView"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "XtraGridColumnsOptions_ListView");
            yield return new NavigationItemAttribute("Controlling XtraGrid/Column options", "XtraGridColumnsOptions_ListView");
            yield return new DisplayFeatureModelAttribute("XtraGridColumnsOptions_ListView");
        }
    }
}
