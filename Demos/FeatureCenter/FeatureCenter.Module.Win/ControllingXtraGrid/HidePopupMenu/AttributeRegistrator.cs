using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.HidePopupMenu {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WinCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderHideGridPopUpMenu, "1=1", "1=1",
                Captions.ViewMessageHideGridPopUpMenu, Position.Bottom) { View = "HidePopupMenu_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderHideGridPopUpMenu, "1=1", "1=1", Captions.HeaderHideGridPopUpMenu
                , Position.Top) { View = "HidePopupMenu_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "HidePopupMenu_ListView");
            yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Hide Popupmenu", "HidePopupMenu_ListView");
            yield return new DisplayFeatureModelAttribute("HidePopupMenu_ListView");
        }
    }
}
