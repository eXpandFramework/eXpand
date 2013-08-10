using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.Navigation.CursorPosition {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WinCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
                Captions.ViewMessageCursorPosition, Position.Bottom) { View = "CursorPosition_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
                Captions.HeaderCursorPosition, Position.Top) { View = "CursorPosition_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "CursorPosition_DetailView");
            yield return new XpandNavigationItemAttribute("Navigation/Cursor Position", "CursorPosition_DetailView");
            yield return new DisplayFeatureModelAttribute("CursorPosition_DetailView");
        }
    }
}
