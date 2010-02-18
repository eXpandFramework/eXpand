using System.ComponentModel;

namespace eXpand.ExpressApp.Logic {
    public class ModelRuleExecutingEventArgs<TModelRuleInfo, TModelRule> : CancelEventArgs where TModelRuleInfo : ModelRuleInfo<TModelRule>, new() where TModelRule : ModelRule {
        readonly ExecutionReason _executionReason;


        public ModelRuleExecutingEventArgs(TModelRuleInfo info, bool cancel, ExecutionReason executionReason)
        {
            _executionReason = executionReason;
            ArtifactStateInfo = info;
            Cancel = cancel;
        }


        /// <summary>
        /// Allows you to customize the information about the artifact states.
        /// </summary>
        public TModelRuleInfo ArtifactStateInfo { get; set; }

        public ExecutionReason ExecutionReason {
            get { return _executionReason; }
        }
    }
}