using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.RuntimeFields.OrphanedCollection {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string RuntimeOrphanedCollection_DetailView = "RuntimeOrphanedCollection_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderOrphanedCollection, "1=1", "1=1", Captions.ViewMessageOrphanedCollection, Position.Bottom) {  View = RuntimeOrphanedCollection_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderOrphanedCollection, "1=1", "1=1",
                Captions.HeaderOrphanedCollection, Position.Top) { View = RuntimeOrphanedCollection_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, RuntimeOrphanedCollection_DetailView);
            yield return new XpandNavigationItemAttribute("Runtime Fields/Orphaned Collection", RuntimeOrphanedCollection_DetailView);
            yield return new DisplayFeatureModelAttribute(RuntimeOrphanedCollection_DetailView, "OrphanedCollection");
        }
    }
}
