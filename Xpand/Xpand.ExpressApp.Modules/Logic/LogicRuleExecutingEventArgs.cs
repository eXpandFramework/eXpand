using System.ComponentModel;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleExecutingEventArgs<TLogicRule> : CancelEventArgs where TLogicRule : ILogicRule {
        readonly ExecutionContext _executionContext;


        public LogicRuleExecutingEventArgs(LogicRuleInfo<TLogicRule> info, bool cancel,
                                           ExecutionContext executionContext) {
            _executionContext = executionContext;
            LogicRuleInfo = info;
            Cancel = cancel;
        }


        /// <summary>
        /// Allows you to customize the information about the artifact states.
        /// </summary>
        public LogicRuleInfo<TLogicRule> LogicRuleInfo { get; set; }

        public ExecutionContext ExecutionContext {
            get { return _executionContext; }
        }
    }
}