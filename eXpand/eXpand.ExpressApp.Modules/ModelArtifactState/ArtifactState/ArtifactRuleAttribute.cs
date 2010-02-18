using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState {
    public abstract class ArtifactRuleAttribute : ModelRuleAttribute, IArtifactRule {
        protected ArtifactRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria,
                                             string emptyCriteria,
                                             ViewType viewType, string module, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, viewId) {
            Module = module;
        }
        #region IArtifactRule Members
        public string Module { get; set; }
        #endregion
    }
}