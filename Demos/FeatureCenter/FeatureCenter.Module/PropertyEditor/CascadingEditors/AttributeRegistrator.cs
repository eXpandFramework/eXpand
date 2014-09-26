using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.PropertyEditor.CascadingEditors {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!ReferenceEquals(typesInfo.Type, typeof(CascadingPropertyEditorObject))) yield break;
            const string cascadingpropertyeditorobjectListview = "CascadingPropertyEditorObject_ListView";
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Captions.PropertyEditors + "Cascading editors", cascadingpropertyeditorobjectListview);
            yield return xpandNavigationItemAttribute;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderCascadingEditors, "1=1", "1=1", Captions.ViewMessageCascadingEditors, Position.Bottom) { ViewType = ViewType.ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderCascadingEditors, "1=1", "1=1",
                Captions.HeaderCascadingEditors, Position.Top);
            yield return new WhatsNewAttribute(new DateTime(2011, 3, 23), xpandNavigationItemAttribute);
        }
    }
}
