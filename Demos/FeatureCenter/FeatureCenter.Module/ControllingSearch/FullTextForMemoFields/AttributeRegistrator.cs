using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.ControllingSearch.FullTextForMemoFields {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.ViewMessageDisableFullTextForMemoFields, Position.Bottom) { ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.HeaderDisableFullTextForMemoFields, Position.Top) { ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView" };
            yield return new CloneModelViewAttribute(CloneViewType.ListView, "DisableFullTextForMemoFields_ListView");
            yield return new XpandNavigationItemAttribute("Controlling Search/Disable Full Text For Memo Fields", "DisableFullTextForMemoFields_ListView");
            yield return new DisplayFeatureModelAttribute("DisableFullTextForMemoFields_ListView");
        }
    }
}
