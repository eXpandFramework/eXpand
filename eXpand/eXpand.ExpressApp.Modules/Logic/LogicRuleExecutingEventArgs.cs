using System.ComponentModel;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    public class LogicRuleExecutingEventArgs<TLogicRule> : CancelEventArgs
        where TLogicRule:ILogicRule
    {
        readonly ExecutionReason _executionReason;


        public LogicRuleExecutingEventArgs(LogicRuleInfo<TLogicRule> info, bool cancel, ExecutionReason executionReason)
        {
            _executionReason = executionReason;
            LogicRuleInfo = info;
            Cancel = cancel;
        }


        /// <summary>
        /// Allows you to customize the information about the artifact states.
        /// </summary>
        public LogicRuleInfo<TLogicRule> LogicRuleInfo { get; set; }

        public ExecutionReason ExecutionReason {
            get { return _executionReason; }
        }
    }
}