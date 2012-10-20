using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.SystemModule;

namespace FeatureCenter.Module.ModelArtifact.ConditionalForeignKeyKViolation {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!Object.Equals(typesInfo.Type, typeof (Customer))) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
                Captions.ViewMessageConditionalFKViolation, Position.Bottom) { View = "ConditionalForeignKeyViolation_ListView", ViewType = ViewType.ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
                Captions.HeaderConditionalFKViolation, Position.Top) { ViewType = ViewType.ListView, View = "ConditionalForeignKeyViolation_ListView" };
            yield return new ControllerStateRuleAttribute("ConditionalForeignKeyViolation", typeof(FKViolationViewController), "City='Paris'", "1=1", ControllerState.Disabled) { View = "ConditionalForeignKeyViolation_ListView" };

            yield return new XpandNavigationItemAttribute("ModelArtifact/Conditional Foreign Key Violation ", "ConditionalForeignKeyViolation_ListView");
            yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalForeignKeyViolation_ListView");
            yield return new DisplayFeatureModelAttribute("ConditionalForeignKeyViolation_ListView");
        }
    }
}
