using System;

namespace Xpand.ExpressApp.Logic {
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class LogicRuleExecuteEventArgs : EventArgs         {
        readonly ExecutionContext _executionContext;


        public LogicRuleExecuteEventArgs(LogicRuleInfo info, ExecutionContext executionContext) {
            _executionContext = executionContext;
            LogicRuleInfo = info;
        }

        public ExecutionContext ExecutionContext {
            get { return _executionContext; }
        }

        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public LogicRuleInfo LogicRuleInfo { get; set; }
        }
}