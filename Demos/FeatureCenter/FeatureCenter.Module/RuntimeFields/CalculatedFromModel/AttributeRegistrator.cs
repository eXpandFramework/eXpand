using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.RuntimeFields.CalculatedFromModel {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string RuntimeCalculatedFieldsFromModelFromModel_ListView = "RuntimeCalculatedFieldsFromModelFromModel_ListView";
        private const string RuntimeCalculatedFieldsFromModelFromModel_DetailView = "RuntimeCalculatedFieldsFromModelFromModel_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRuntimeCalculatedFieldsFromModel, "1=1", "1=1", Captions.ViewMessageRuntimeCalculatedFieldsFromModel, Position.Bottom) { ViewType = ViewType.ListView, View = RuntimeCalculatedFieldsFromModelFromModel_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRuntimeCalculatedFieldsFromModel, "1=1", "1=1",
                Captions.HeaderRuntimeCalculatedFieldsFromModel, Position.Top) { View = RuntimeCalculatedFieldsFromModelFromModel_ListView };
            yield return new CloneModelViewAttribute(CloneViewType.DetailView, RuntimeCalculatedFieldsFromModelFromModel_DetailView);
            yield return new CloneModelViewAttribute(CloneViewType.ListView, RuntimeCalculatedFieldsFromModelFromModel_ListView) { DetailView = RuntimeCalculatedFieldsFromModelFromModel_DetailView };
            yield return new XpandNavigationItemAttribute("Runtime Fields/Calculated Using Model", RuntimeCalculatedFieldsFromModelFromModel_ListView);
            yield return new DisplayFeatureModelAttribute(RuntimeCalculatedFieldsFromModelFromModel_ListView, "CalculatedFromModel");
            yield return new DisplayFeatureModelAttribute(RuntimeCalculatedFieldsFromModelFromModel_DetailView, "CalculatedFromModel");
        }
    }
}
