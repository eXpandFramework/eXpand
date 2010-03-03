using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic{
    public abstract class LogicRuleViewController<TLogicRule> : ViewController where TLogicRule : ILogicRule{

        private bool isRefreshing;
        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas" + typeof(TLogicRule).Name;
        
        public virtual bool IsReady{
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext){
            if (isReady){
                object currentObject = view.CurrentObject;
                foreach (TLogicRule rule in LogicRuleManager<TLogicRule>.Instance[view.ObjectTypeInfo])
                    if (IsValidRule(rule, view))
                        ForceExecutionCore(currentObject, rule, invertCustomization, view, executionContext);
            }
        }

        public abstract void ExecuteRule(LogicRuleInfo<TLogicRule> info, ExecutionContext executionContext);


        
        public event EventHandler<LogicRuleExecutingEventArgs<TLogicRule>> LogicRuleExecuting;

        
        public event EventHandler<LogicRuleExecutedEventArgs<TLogicRule>> LogicRuleExecuted;
        

        private void FrameOnViewChanging(object sender, EventArgs args){
            if (View != null) InvertExecution(View);
            var view = ((ViewChangingEventArgs)args).View;
            Active[ActiveObjectTypeHasRules] = LogicRuleManager<TLogicRule>.HasRules(view);
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view, false, ExecutionContext.ViewChanging);
            
        }

        private void InvertExecution(View view){
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view,true, ExecutionContext.ViewChanging);
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewControlAdding = (Frame.Template) as ISupportViewControlAdding;
            if (supportViewControlAdding != null)
                supportViewControlAdding.ViewControlAdding += (o, args) => ForceExecution(ExecutionContext.ViewControlAdding); 
        }

        protected override void OnActivated(){base.OnActivated();
            if (IsReady){
                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                ForceExecution(ExecutionContext.ViewActivated);
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            }
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            ForceExecution(ExecutionContext.ViewControlsCreated);
        }
        protected override void OnDeactivating(){
            base.OnDeactivating();
            if (IsReady){
                Frame.TemplateViewChanged-=FrameOnTemplateViewChanged;
                View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            }
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e){
            isRefreshing = false;
            ForceExecution(ExecutionContext.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e){
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            if (!isRefreshing){
                ForceExecution(ExecutionContext.CurrentObjectChanged);
            }
        }

        private void ForceExecution(ExecutionContext executionContext,View view) {
            ForceExecution(IsReady, view, false, executionContext);
        }

        private void ForceExecution(ExecutionContext executionContext){
            ForceExecution(executionContext,View);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args){
            if (!String.IsNullOrEmpty(args.PropertyName)){
                ForceExecution(ExecutionContext.ObjectChanged);
            }
        }

        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs<TLogicRule> args){
            if (LogicRuleExecuting != null){
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecutedEventArgs<TLogicRule> args){
            if (LogicRuleExecuted != null){
                LogicRuleExecuted(this, args);
            }
        }

        protected virtual bool IsValidRule(TLogicRule rule, View view){
            return view != null &&(string.IsNullOrEmpty(rule.ViewId)||view.Id==rule.ViewId)&& view.ObjectTypeInfo != null &&
                   IsValidViewType(view, rule) && IsValidNestedType(rule,view)&&(rule.TypeInfo.IsAssignableFrom(view.ObjectTypeInfo));
        }

        private bool IsValidNestedType(TLogicRule rule, View view) {
            return view is DetailView || (rule.Nesting == Nesting.Any || view.IsRoot);
        }

        private bool IsValidViewType(View view, TLogicRule rule){
            return (rule.ViewType == ViewType.Any || (view is DetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType==ViewType.ListView));
        }

        protected virtual LogicRuleInfo<TLogicRule> CalculateLogicRuleInfo(object targetObject, TLogicRule logicRule){
            var logicRuleInfo = (LogicRuleInfo<TLogicRule>)Activator.CreateInstance(typeof(LogicRuleInfo<TLogicRule>));
            logicRuleInfo.Active = true;
            logicRuleInfo.Object = targetObject;
            logicRuleInfo.Rule = logicRule;
            logicRuleInfo.ExecutionContext = LogicRuleManager<TLogicRule>.GetExecutionContext(logicRule.ExecutionContextGroup);
            return logicRuleInfo;

        }

        

        protected virtual void ForceExecutionCore(object currentObject, TLogicRule rule, bool invertCustomization, View view, ExecutionContext executionContext){
            LogicRuleInfo<TLogicRule> info = CalculateLogicRuleInfo(currentObject, rule);
            if (info != null&&ContextIsValid(executionContext,info)){
                info.InvertingCustomization = invertCustomization;
                info.View = view;
                if (invertCustomization)
                    info.Active = !info.Active;
                var args = new LogicRuleExecutingEventArgs<TLogicRule>(info, false, executionContext);
                OnLogicRuleExecuting(args);
                if (!args.Cancel){
                    ExecuteRule(info, executionContext);
                }
                OnLogicRuleExecuted(new LogicRuleExecutedEventArgs<TLogicRule>(info, executionContext));
            }
        }

        public virtual bool ContextIsValid(ExecutionContext executionContext, LogicRuleInfo<TLogicRule> logicRuleInfo) {
            return (logicRuleInfo.ExecutionContext | executionContext) == executionContext;
        }
    }
}