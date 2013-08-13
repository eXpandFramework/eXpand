using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.Conditional.Security;

namespace eXpand.ExpressApp.ConditionalActionState.Security {
    public abstract class ArtifactStateRulePermission : ConditionalLogicRulePermission, IArtifactStateRule
    {
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}