using System;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleExecutor {
        readonly LogicRuleEvaluator _evaluator = new LogicRuleEvaluator();
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecuted;
        public event EventHandler<LogicRuleExecutingEventArgs> LogicRuleExecuting;
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecute;

        public LogicRuleEvaluator Evaluator {
            get { return _evaluator; }
        }

        public virtual void Execute<T>(View view, bool invertCustomization, ExecutionContext executionContext,
            object currentObject, EventArgs eventArgs) where T:ILogicRule{
            var logicRuleObjects = _evaluator.GetValidRules(view, executionContext, eventArgs);
            var validRules = logicRuleObjects.OfType<T>().Cast<ILogicRuleObject>();
            var logicRuleInfos = validRules.Select(o => new LogicRuleInfo {
                Active = _evaluator.Fit(currentObject, o),
                Object = currentObject,
                Rule = o,
                ExecutionContext = executionContext,
                View = view,
                EventArgs = eventArgs,
                InvertCustomization = invertCustomization
            });
            foreach (var logicRuleInfo in logicRuleInfos) {
                ExecuteCore(logicRuleInfo, executionContext);
            }
        }

        internal virtual void Execute(View view, bool invertCustomization, ExecutionContext executionContext, object currentObject, EventArgs eventArgs) {
            Execute<ILogicRule>(view, invertCustomization, executionContext, currentObject, eventArgs);
        }

        void ExecuteCore(LogicRuleInfo logicRuleInfo, ExecutionContext executionContext) {
            var args = new LogicRuleExecutingEventArgs(logicRuleInfo, false, executionContext);
            OnLogicRuleExecuting(args);
            if (!args.Cancel) {
                OnLogicRuleExecute(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
            }
            OnLogicRuleExecuted(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
        }

        protected virtual void OnLogicRuleExecute(LogicRuleExecuteEventArgs e) {
            EventHandler<LogicRuleExecuteEventArgs> handler = LogicRuleExecute;
            if (handler != null) handler(this, e);
        }

        public void InvertExecution(View view, ExecutionContext executionContext, object currentObject,EventArgs eventArgs) {
            Execute(view, true, executionContext, currentObject,eventArgs);
        }

        protected void InvertExecution(View view, ExecutionContext executionContext, EventArgs args, View oldView) {
            InvertExecution(view, executionContext, oldView.CurrentObject,args);
        }

        public void InvertAndExecute(View view, ExecutionContext executionContext, EventArgs eventArgs,View oldView) {
            if (oldView != null) InvertExecution(oldView, executionContext, eventArgs,oldView);
            Execute(executionContext, view, eventArgs);
        }


        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs args) {
            if (LogicRuleExecuting != null) {
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecuteEventArgs args) {
            if (args.LogicRuleInfo.Rule.IsNew.HasValue)
                Evaluator.IsNew = false;
            if (LogicRuleExecuted != null) {
                LogicRuleExecuted(this, args);
            }
        }

        public void Execute<T>(ExecutionContext executionContext, View view) where T : ILogicRule{
            Execute<T>(executionContext, view,EventArgs.Empty);
        }

        public void Execute<T>(ExecutionContext executionContext, View view, EventArgs eventArgs) where T:ILogicRule{
            Execute<T>(view, false, executionContext, view != null ? view.CurrentObject : null, eventArgs);
        }

        internal void Execute(ExecutionContext executionContext, View view, EventArgs eventArgs) {
            Execute<ILogicRule>(executionContext, view, eventArgs);
        }

        public void Execute<T>(ExecutionContext executionContext, EventArgs eventArgs, View view) where T : ILogicRule {
            Execute<T>(executionContext, view, eventArgs);
        }

        internal void Execute(ExecutionContext executionContext,EventArgs eventArgs,View view) {
            Execute<ILogicRule>(executionContext, view, eventArgs);
        }
    }
}