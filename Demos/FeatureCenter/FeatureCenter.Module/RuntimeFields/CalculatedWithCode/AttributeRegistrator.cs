using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.RuntimeFields.CalculatedWithCode {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string RuntimeCalculatedFields_ListView = "RuntimeCalculatedFields_ListView";
        private const string RuntimeCalculatedFields_DetailView = "RuntimeCalculatedFields_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRuntimeCalculatedFields, "1=1", "1=1", Captions.ViewMessageRuntimeCalculatedFields, Position.Bottom) { ViewType = ViewType.ListView, View = RuntimeCalculatedFields_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRuntimeCalculatedFields, "1=1", "1=1",
                Captions.HeaderRuntimeCalculatedFields, Position.Top) { View = RuntimeCalculatedFields_ListView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, RuntimeCalculatedFields_DetailView);
            yield return new CloneViewAttribute(CloneViewType.ListView, RuntimeCalculatedFields_ListView) { DetailView = RuntimeCalculatedFields_DetailView };
            yield return new XpandNavigationItemAttribute("Runtime Fields/Calculated using code", RuntimeCalculatedFields_ListView);
            yield return new DisplayFeatureModelAttribute(RuntimeCalculatedFields_ListView, "RuntimeFieldsCalculatedWithCode");
            yield return new DisplayFeatureModelAttribute(RuntimeCalculatedFields_DetailView, "RuntimeFieldsCalculatedWithCode");
        }
    }
}
