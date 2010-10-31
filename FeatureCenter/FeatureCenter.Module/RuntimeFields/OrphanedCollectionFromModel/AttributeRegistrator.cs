using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.RuntimeFields.OrphanedCollectionFromModel {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string RuntimeOrphanedCollection_DetailView = "RuntimeOrphanedCollectionFromModel_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderOrphanedCollectionFromModel, "1=1", "1=1", Captions.ViewMessageOrphanedCollectionFromModel, Position.Bottom) {  View = RuntimeOrphanedCollection_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderOrphanedCollectionFromModel, "1=1", "1=1",
                Captions.HeaderOrphanedCollectionFromModel, Position.Top) { View = RuntimeOrphanedCollection_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, RuntimeOrphanedCollection_DetailView);
            yield return new XpandNavigationItemAttribute("Runtime Fields/Orphaned Collection from model", RuntimeOrphanedCollection_DetailView);
            yield return new DisplayFeatureModelAttribute(RuntimeOrphanedCollection_DetailView, "OrphanedCollectionFromModel");
        }
    }
}
