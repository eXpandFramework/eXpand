using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.PropertyEditors.StringPropertyEditors {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(SPECustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderStringPropertyEditors, "1=1", "1=1",
                Captions.ViewMessageStringPropertyEditors, Position.Bottom) { View = "StringPropertyEditors_DetailView", NotUseSameType = true };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderStringPropertyEditors + "1", "1=1", "1=1",
                Captions.ViewMessageStringPropertyEditors1, Position.Bottom) { View = "StringPropertyEditors_DetailView", NotUseSameType = true };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderStringPropertyEditors, "1=1", "1=1",
                Captions.HeaderStringPropertyEditors, Position.Top) { View = "StringPropertyEditors_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "StringPropertyEditors_DetailView");
            yield return new XpandNavigationItemAttribute(Module.Captions.PropertyEditors + "String editors", "StringPropertyEditors_DetailView");
            yield return new DisplayFeatureModelAttribute("StringPropertyEditors_DetailView");
            yield return new WhatsNewAttribute("28/1/2011");
        }
    }
}
