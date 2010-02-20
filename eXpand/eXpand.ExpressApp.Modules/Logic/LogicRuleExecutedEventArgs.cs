using System;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class LogicRuleExecutedEventArgs<TLogicRule> : EventArgs
        where TLogicRule:ILogicRule
    {
        readonly ExecutionReason _executionReason;


        public LogicRuleExecutedEventArgs(LogicRuleInfo<TLogicRule> info, ExecutionReason executionReason)
        {
            _executionReason = executionReason;
            LogicRuleInfo = info;
        }

        public ExecutionReason ExecutionReason {
            get { return _executionReason; }
        }

        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public LogicRuleInfo<TLogicRule> LogicRuleInfo { get; set; }
    }
}