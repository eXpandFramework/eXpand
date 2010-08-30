using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.GuessAutoFilterRowValues
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(WinCustomer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
                Captions.ViewMessageGuessAutoFilterRowValuesFromFilter, Position.Bottom){View = "GuessAutoFilterRowValues_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
                Captions.HeaderGuessAutoFilterRowValuesFromFilter, Position.Top){View = "GuessAutoFilterRowValues_ListView"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "GuessAutoFilterRowValues_ListView");
            yield return new NavigationItemAttribute("Controlling XtraGrid/Guess Auto FilterRow Values", "GuessAutoFilterRowValues_ListView");
            new DisplayFeatureModelAttribute("GuessAutoFilterRowValues_ListView");
        }
    }
}
