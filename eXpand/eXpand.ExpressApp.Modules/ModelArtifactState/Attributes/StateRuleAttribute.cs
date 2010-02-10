using DevExpress.ExpressApp;
using eXpand.ExpressApp.RuleModeller;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes {
    public class StateRuleAttribute : ModelRuleAttribute, IStateRule {
        State _state;

        public StateRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria, string emptyCriteria,
                                  ViewType viewType, State state, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, viewId) {
            _state = state;
        }
        #region IStateRule Members
        public virtual State State {
            get { return _state; }
            set { _state = value; }
        }
        #endregion
    }
}