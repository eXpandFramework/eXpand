using System;

namespace eXpand.ExpressApp.Logic {
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class ModelRuleExecutedEventArgs<TModelRuleInfo, TModelRule> : EventArgs where TModelRuleInfo : ModelRuleInfo<TModelRule>, new() where TModelRule : ModelRule {
        readonly ExecutionReason _executionReason;


        public ModelRuleExecutedEventArgs(TModelRuleInfo info, ExecutionReason executionReason) {
            _executionReason = executionReason;
            ArtifactStateInfo = info;
        }

        public ExecutionReason ExecutionReason {
            get { return _executionReason; }
        }

        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public TModelRuleInfo ArtifactStateInfo { get; set; }
    }
}