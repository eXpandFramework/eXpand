using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.SystemModule;

namespace FeatureCenter.Module.ModelArtifact.ConditionalForeignKeyKViolationAndShowDetailView
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
                Captions.ViewMessageConditionalFKViolation, Position.Bottom){View = "ConditionalForeignKeyViolationAndShowDetailView_ListView", NotUseSameType = true,ViewType = ViewType.ListView};
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation + "2", "1=1", "1=1",
                Captions.ViewMessageConditionalFKViolation2, Position.Bottom){ViewType = ViewType.ListView,View = "ConditionalForeignKeyViolationAndShowDetailView_ListView", NotUseSameType = true};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
                Captions.HeaderConditionalFKViolation, Position.Top){ViewType = ViewType.ListView,View = "ConditionalForeignKeyViolationAndShowDetailView_ListView"};
            yield return new ControllerStateRuleAttribute("ConditionalForeignKeyViolation", typeof(FKViolationViewController),"City='Paris'", "1=1",ControllerState.Disabled)
                {View = "ConditionalForeignKeyViolationAndShowDetailView_ListView"};
            yield return new ControllerStateRuleAttribute("ConditionalShowDetailView", typeof(ListViewProcessCurrentObjectController), "City='Paris'",
                "1=1", ControllerState.Disabled){View = "ConditionalForeignKeyViolationAndShowDetailView_ListView"};
            yield return new NavigationItemAttribute("ModelArtifact/Conditional Foreign Key Violation And Show DetailView", "ConditionalForeignKeyViolationAndShowDetailView_ListView");
            yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalForeignKeyViolationAndShowDetailView_ListView");
            yield return new DisplayFeatureModelAttribute("ConditionalForeignKeyViolationAndShowDetailView_ListView");
        }
    }
}
