using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PropertyEditor.CascadingEditors {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!Object.ReferenceEquals(typesInfo.Type, typeof(CascadingPropertyEditorObject))) yield break;
            yield return new XpandNavigationItemAttribute(Captions.PropertyEditors + "Cascading editors", "CascadingPropertyEditorObject_DetailView");
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderCascadingEditors, "1=1", "1=1", Captions.ViewMessageCascadingEditors, Position.Bottom) { ViewType = ViewType.DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderCascadingEditors, "1=1", "1=1",
                Captions.HeaderCascadingEditors, Position.Top);

        }
    }
}
