using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security.Improved;

namespace Xpand.ExpressApp.ArtifactState.Security.Improved {
    public abstract class ArtifactStateRulePermission : ConditionalLogicRulePermission, IArtifactStateRule {
        protected ArtifactStateRulePermission(string operation, IArtifactStateRule logicRule)
            : base(operation, logicRule) {
            Module = logicRule.Module;
        }
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}
