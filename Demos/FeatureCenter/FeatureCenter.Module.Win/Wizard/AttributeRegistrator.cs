using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.Wizard {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public static readonly string WizardCustomer_DetailView = typeof(WizardCustomer).Namespace+".WizardCustomer_DetailView";

        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WizardCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderWizard, "1=1", "1=1", Captions.HeaderWizard, Position.Top) { View = WizardCustomer_DetailView + 1 };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderWizard, "1=1", "1=1",
                Captions.ViewMessageWizard, Position.Bottom) { View = WizardCustomer_DetailView + 1 };
            yield return new CloneViewAttribute(CloneViewType.DetailView, WizardCustomer_DetailView + 1);
            yield return new CloneViewAttribute(CloneViewType.DetailView, WizardCustomer_DetailView + 2);
            var navigationItemAttribute = new XpandNavigationItemAttribute(Module.Captions.DetailViewCotrol + "Wizard", WizardCustomer_DetailView);
            yield return navigationItemAttribute;
            yield return new WhatsNewAttribute(new DateTime(2012,3,16), navigationItemAttribute);
            yield return new DisplayFeatureModelAttribute(WizardCustomer_DetailView);
        }
    }
}
