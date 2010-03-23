using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactRuleAttribute : ConditionalLogicRuleAttribute, IArtifactRule {
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