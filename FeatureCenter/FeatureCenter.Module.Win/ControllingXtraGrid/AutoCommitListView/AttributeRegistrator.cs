using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.AutoCommitListView
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
                Captions.ViewMessageAutoCommitListView, Position.Bottom){View = "AutoCommit_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
                Captions.HeaderAutoCommitListView, Position.Top){View = "AutoCommit_ListView"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "AutoCommit_ListView");
            yield return new NavigationItemAttribute("Controlling XtraGrid/Auto Commit List View", "AutoCommit_ListView");
            yield return new DisplayFeatureModelAttribute("AutoCommit_ListView", "AutoCommitListView");
        }
    }
}
