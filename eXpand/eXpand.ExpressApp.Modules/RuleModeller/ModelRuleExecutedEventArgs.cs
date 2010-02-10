using System;

namespace eXpand.ExpressApp.RuleModeller {
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class ModelRuleExecutedEventArgs<TModelRuleInfo, TModelRule> : EventArgs where TModelRuleInfo : ModelRuleInfo<TModelRule>, new() where TModelRule : ModelRule {


        public ModelRuleExecutedEventArgs(TModelRuleInfo info, ExecutionReason executionReason)
        {
            ArtifactStateInfo = info;
        }


        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public TModelRuleInfo ArtifactStateInfo { get; set; }
    }
}