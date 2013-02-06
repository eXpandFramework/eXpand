using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.RuntimeFields.DC {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public const string RuntimeFieldsFromModel_ListView = "RuntimeFieldsFromModelForDC_ListView";
        private const string RuntimeFieldsFromModel_DetailView = "RuntimeFieldsFromModelForDC_DetailView";
        private const string RuntimeFieldsFromModel = "RuntimeFieldsFromModelForDC";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(DomainComponentWithRuntimeMembers)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRuntimeMemberFromModelForDC, "1=1", "1=1", Captions.ViewMessageRuntimeMemberFromModelForDC, Position.Bottom) { ViewType = ViewType.ListView, View = RuntimeFieldsFromModel_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRuntimeMemberFromModelForDC, "1=1", "1=1",
                Captions.HeaderRuntimeMemberFromModelForDC, Position.Top) { View = RuntimeFieldsFromModel_ListView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, RuntimeFieldsFromModel_DetailView);
            yield return new CloneViewAttribute(CloneViewType.ListView, RuntimeFieldsFromModel_ListView) { DetailView = RuntimeFieldsFromModel_DetailView };
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("Runtime Fields/From Model For DC", RuntimeFieldsFromModel_ListView);
            yield return xpandNavigationItemAttribute;
            yield return new DisplayFeatureModelAttribute(RuntimeFieldsFromModel_ListView, RuntimeFieldsFromModel);
            yield return new DisplayFeatureModelAttribute(RuntimeFieldsFromModel_DetailView, RuntimeFieldsFromModel);
            yield return new WhatsNewAttribute(new DateTime(2013, 2, 5), xpandNavigationItemAttribute);
        }
    }
}
