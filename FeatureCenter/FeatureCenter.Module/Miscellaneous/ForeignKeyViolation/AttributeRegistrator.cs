using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.ForeignKeyViolation
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.ViewMessageFKViolation, Position.Bottom){ViewType = ViewType.ListView, View = "ForeignKeyViolation_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.HeaderFKViolation, Position.Top) { View = "ForeignKeyViolation_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "ForeignKeyViolation_ListView");
            yield return new XpandNavigationItemAttribute(Captions.Miscellaneous + "Foreign Key Violation", "ForeignKeyViolation_ListView");
            yield return new DisplayFeatureModelAttribute("ForeignKeyViolation_ListView");
        }
    }
}
