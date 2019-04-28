using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.AutoCommitListView {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WinCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
                Captions.ViewMessageAutoCommitListView, Position.Bottom) { View = "AutoCommit_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
                Captions.HeaderAutoCommitListView, Position.Top) { View = "AutoCommit_ListView" };
            yield return new CloneModelViewAttribute(CloneViewType.ListView, "AutoCommit_ListView");
            yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Auto Commit List View", "AutoCommit_ListView");
            yield return new DisplayFeatureModelAttribute("AutoCommit_ListView", "AutoCommitListView");
        }
    }
}
