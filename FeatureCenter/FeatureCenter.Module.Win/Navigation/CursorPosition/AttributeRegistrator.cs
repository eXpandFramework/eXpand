using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Navigation.CursorPosition
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
                Captions.ViewMessageCursorPosition, Position.Bottom){View = "CursorPosition_DetailView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
                Captions.HeaderCursorPosition, Position.Top) { View = "CursorPosition_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "CursorPosition_DetailView");
            yield return new NavigationItemAttribute("Navigation/Cursor Position", "CursorPosition_DetailView");
            yield return new DisplayFeatureModelAttribute("CursorPosition_DetailView");
        }
    }
}
