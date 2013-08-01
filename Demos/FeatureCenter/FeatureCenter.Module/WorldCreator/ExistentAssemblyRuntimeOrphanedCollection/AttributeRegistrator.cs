using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeOrphanedCollection {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string ExistentAssemblyRuntimeOrphanedCollection_DetailView = "ExistentAssemblyRuntimeOrphanedCollection_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderExistentAssemblyRuntimeOrphanedCollection, "1=1", "1=1", Captions.ViewMessageExistentAssemblyRuntimeOrphanedCollection, Position.Bottom) {  View = ExistentAssemblyRuntimeOrphanedCollection_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderExistentAssemblyRuntimeOrphanedCollection, "1=1", "1=1", Captions.HeaderExistentAssemblyRuntimeOrphanedCollection, Position.Top) { View = ExistentAssemblyRuntimeOrphanedCollection_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, ExistentAssemblyRuntimeOrphanedCollection_DetailView);
            yield return new XpandNavigationItemAttribute("WorldCreator/Existent Assembly/Orphaned Collection", ExistentAssemblyRuntimeOrphanedCollection_DetailView);
            yield return new DisplayFeatureModelAttribute(ExistentAssemblyRuntimeOrphanedCollection_DetailView, "ExistentAssemblyRuntimeOrphanedCollection");
        }
    }
}
