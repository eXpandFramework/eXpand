using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeCalculatedField {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string ExistentAssemblyRuntimeFields_DetailView = "ExistentAssemblyRuntimeFields_DetailView";
        private const string ExistentAssemblyRuntimeFields_ListView = "ExistentAssemblyRuntimeFields_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderExistentAssemblyRuntimeCalculatedField, "1=1", "1=1", Captions.ViewMessageExistentAssemblyRuntimeCalculatedField, Position.Bottom) { ViewType = ViewType.ListView, View = ExistentAssemblyRuntimeFields_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderExistentAssemblyRuntimeCalculatedField, "1=1", "1=1", Captions.HeaderExistentAssemblyRuntimeCalculatedField, Position.Top) { View = ExistentAssemblyRuntimeFields_ListView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, ExistentAssemblyRuntimeFields_DetailView);
            yield return new CloneViewAttribute(CloneViewType.ListView, ExistentAssemblyRuntimeFields_ListView) { DetailView = ExistentAssemblyRuntimeFields_DetailView };
            yield return new XpandNavigationItemAttribute("WorldCreator/Existent Assembly/Calculated Fields", ExistentAssemblyRuntimeFields_ListView);
            yield return new DisplayFeatureModelAttribute(ExistentAssemblyRuntimeFields_ListView, "ExistentAssemblyRuntimeCalculatedField");
            yield return new DisplayFeatureModelAttribute(ExistentAssemblyRuntimeFields_DetailView, "ExistentAssemblyRuntimeCalculatedField");
        }
    }
}
