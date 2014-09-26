using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.Validation {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        const string RuleType_ListView = "RuleTypeCustomer_ListView";

        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(RuleTypeCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderValidation, "1=1", "1=1", Captions.ViewMessageValidation, Position.Bottom) { ViewType = ViewType.ListView, View = RuleType_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderValidation, "1=1", "1=1",
                Captions.HeaderValidation, Position.Top) { View = RuleType_ListView };
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("Validation/Rule Types", RuleType_ListView);
            yield return xpandNavigationItemAttribute;
            yield return new WhatsNewAttribute(new DateTime(2011, 10, 23), xpandNavigationItemAttribute);
            yield return new DisplayFeatureModelAttribute(RuleType_ListView, "RuleType");
        }
    }
}
