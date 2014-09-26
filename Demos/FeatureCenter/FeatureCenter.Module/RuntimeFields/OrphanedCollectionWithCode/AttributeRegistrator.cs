using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.RuntimeFields.OrphanedCollectionWithCode {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string RuntimeOrphanedCollection_DetailView = "RuntimeOrphanedCollectionWithCode_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderOrphanedCollectionWithCode, "1=1", "1=1", Captions.ViewMessageOrphanedCollectionWithCode, Position.Bottom) { View = RuntimeOrphanedCollection_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderOrphanedCollectionWithCode, "1=1", "1=1",
                Captions.HeaderOrphanedCollectionWithCode, Position.Top) { View = RuntimeOrphanedCollection_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, RuntimeOrphanedCollection_DetailView);
            yield return new XpandNavigationItemAttribute("Runtime Fields/Orphaned Collection With Code", RuntimeOrphanedCollection_DetailView);
            yield return new DisplayFeatureModelAttribute(RuntimeOrphanedCollection_DetailView, "OrphanedCollectionWithCode");
        }
    }
}
