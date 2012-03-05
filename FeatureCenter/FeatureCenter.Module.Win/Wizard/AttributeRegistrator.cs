using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Wizard {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public const string Wizard_DetailView = "Wizard_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderWizard, "1=1", "1=1", Captions.HeaderWizard, Position.Top) { View = Wizard_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderWizard, "1=1", "1=1",
                Captions.ViewMessageWizard, Position.Bottom) { View = Wizard_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, Wizard_DetailView);
            yield return new CloneViewAttribute(CloneViewType.DetailView, "Wizard_DetailView1");
            yield return new XpandNavigationItemAttribute(Module.Captions.DetailViewCotrol + "Wizard", Wizard_DetailView);
            yield return new DisplayFeatureModelAttribute(Wizard_DetailView);
        }
    }
}
