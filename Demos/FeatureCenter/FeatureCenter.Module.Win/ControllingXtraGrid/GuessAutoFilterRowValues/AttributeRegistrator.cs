using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.GuessAutoFilterRowValues {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WinCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
                Captions.ViewMessageGuessAutoFilterRowValuesFromFilter, Position.Bottom) { View = "GuessAutoFilterRowValues_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
                Captions.HeaderGuessAutoFilterRowValuesFromFilter, Position.Top) { View = "GuessAutoFilterRowValues_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "GuessAutoFilterRowValues_ListView");
            yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Guess Auto FilterRow Values", "GuessAutoFilterRowValues_ListView");
            yield return new DisplayFeatureModelAttribute("GuessAutoFilterRowValues_ListView");
        }
    }
}
