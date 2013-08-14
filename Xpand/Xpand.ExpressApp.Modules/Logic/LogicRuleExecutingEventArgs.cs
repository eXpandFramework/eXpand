using System.ComponentModel;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleExecutingEventArgs : CancelEventArgs  {
        readonly ExecutionContext _executionContext;


        public LogicRuleExecutingEventArgs(LogicRuleInfo info, bool cancel,
                                           ExecutionContext executionContext) {
            _executionContext = executionContext;
            LogicRuleInfo = info;
            Cancel = cancel;
        }


        /// <summary>
        /// Allows you to customize the information about the artifact states.
        /// </summary>
        public LogicRuleInfo LogicRuleInfo { get; set; }

        public ExecutionContext ExecutionContext {
            get { return _executionContext; }
        }
    }
}