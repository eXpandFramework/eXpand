using System;

namespace eXpand.ExpressApp.Logic {
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class LogicRuleExecutedEventArgs<TLogicRule> : EventArgs
        where TLogicRule : ILogicRule {
        readonly ExecutionContext _executionContext;


        public LogicRuleExecutedEventArgs(LogicRuleInfo<TLogicRule> info, ExecutionContext executionContext) {
            _executionContext = executionContext;
            LogicRuleInfo = info;
        }

        public ExecutionContext ExecutionContext {
            get { return _executionContext; }
        }

        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public LogicRuleInfo<TLogicRule> LogicRuleInfo { get; set; }
        }
}