using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.Miscellaneous.ForeignKeyViolation {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.ViewMessageFKViolation, Position.Bottom) { ViewType = ViewType.ListView, View = "ForeignKeyViolation_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.HeaderFKViolation, Position.Top) { View = "ForeignKeyViolation_ListView" };
            yield return new CloneModelViewAttribute(CloneViewType.ListView, "ForeignKeyViolation_ListView");
            yield return new XpandNavigationItemAttribute(Captions.Miscellaneous + "Foreign Key Violation", "ForeignKeyViolation_ListView");
            yield return new DisplayFeatureModelAttribute("ForeignKeyViolation_ListView");
        }
    }
}
