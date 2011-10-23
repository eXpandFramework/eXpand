using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Validation {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        
        private const string Validation_ListView = "Warning_ListView";
        private const string Validation_DetailView = "Warning_DetailView";

        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WarningCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderValidation, "1=1", "1=1", Captions.ViewMessageValidation, Position.Bottom) { ViewType = ViewType.ListView, View = Validation_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderValidation, "1=1", "1=1",
                Captions.HeaderValidation, Position.Top) { View = Validation_ListView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, Validation_DetailView);
            yield return new CloneViewAttribute(CloneViewType.ListView, Validation_ListView) { DetailView = Validation_DetailView };
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("Validation/Warnings", Validation_ListView);
            yield return xpandNavigationItemAttribute;
            yield return new WhatsNewAttribute(new DateTime(2011, 10, 23), xpandNavigationItemAttribute);
            yield return new DisplayFeatureModelAttribute(Validation_ListView, "Validation");
        }
    }
}
