using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.Logic{
    public abstract class LogicRuleViewController<TModelLogicRule> : ViewController where TModelLogicRule : ILogicRule{

        private bool isRefreshing;
        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas" + typeof(TModelLogicRule).Name;

        public virtual bool IsReady{
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext){
            if (isReady){
                object currentObject = view.CurrentObject;
                foreach (TModelLogicRule rule in LogicRuleManager<TModelLogicRule>.Instance[view.ObjectTypeInfo])
                    if (IsValidRule(rule, view))
                        ForceExecutionCore(currentObject, rule, invertCustomization, view, executionContext);
            }
        }

        public abstract void ExecuteRule(LogicRuleInfo<TModelLogicRule> info, ExecutionContext executionContext);


        
        public event EventHandler<LogicRuleExecutingEventArgs<TModelLogicRule>> LogicRuleExecuting;

        
        public event EventHandler<LogicRuleExecutedEventArgs<TModelLogicRule>> LogicRuleExecuted;
        

        private void FrameOnViewChanging(object sender, EventArgs args){
            if (View != null) InvertExecution(View);
            var view = ((ViewChangingEventArgs)args).View;
            Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(view);
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

        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs<TModelLogicRule> args){
            if (LogicRuleExecuting != null){
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecutedEventArgs<TModelLogicRule> args){
            if (LogicRuleExecuted != null){
                LogicRuleExecuted(this, args);
            }
        }

        protected virtual bool IsValidRule(TModelLogicRule rule, View view){
            return view != null &&(IsValidViewId(view, rule))&&
                   IsValidViewType(view, rule) && IsValidNestedType(rule, view) && IsValidTypeInfo(view, rule);
        }

        bool IsValidViewId(View view, TModelLogicRule rule) {
            return string.IsNullOrEmpty(rule.ViewId)||view.Id==rule.ViewId;
        }

        bool IsValidTypeInfo(View view, TModelLogicRule rule) {
            return ((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(view.ObjectTypeInfo)) || rule.TypeInfo == null);
        }

        private bool IsValidNestedType(TModelLogicRule rule, View view) {
            return view is DetailView || (rule.Nesting == Nesting.Any || view.IsRoot);
        }

        private bool IsValidViewType(View view, TModelLogicRule rule){
            return (rule.ViewType == ViewType.Any || (view is DetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType==ViewType.ListView));
        }

        protected virtual LogicRuleInfo<TModelLogicRule> CalculateLogicRuleInfo(object targetObject, TModelLogicRule modelLogicRule){
            var logicRuleInfo = (LogicRuleInfo<TModelLogicRule>)Activator.CreateInstance(typeof(LogicRuleInfo<TModelLogicRule>));
            logicRuleInfo.Active = true;
            logicRuleInfo.Object = targetObject;
            logicRuleInfo.Rule = modelLogicRule;

            logicRuleInfo.ExecutionContext = CalculateCurrentExecutionContext(modelLogicRule.ExecutionContextGroup);
            return logicRuleInfo;
        }

        ExecutionContext CalculateCurrentExecutionContext(string executionContextGroup) {
            var modelGroupContexts = GetModelGroupContexts(executionContextGroup);
            IModelIModelExecutionContexts iModelExecutionContexts = modelGroupContexts.Where(context => context.Id == executionContextGroup).Single();
            return iModelExecutionContexts.Aggregate(ExecutionContext.None, (current, modelGroupContext) =>
                                                current | (ExecutionContext)Enum.Parse(typeof(ExecutionContext), modelGroupContext.Id.ToString()));    
        }

        protected abstract IModelGroupContexts GetModelGroupContexts(string executionContextGroup);
        


        protected virtual void ForceExecutionCore(object currentObject, TModelLogicRule rule, bool invertCustomization, View view, ExecutionContext executionContext){
            LogicRuleInfo<TModelLogicRule> info = CalculateLogicRuleInfo(currentObject, rule);
            if (info != null&&ContextIsValid(executionContext,info)){
                info.InvertingCustomization = invertCustomization;
                info.View = view;
                if (invertCustomization)
                    info.Active = !info.Active;
                var args = new LogicRuleExecutingEventArgs<TModelLogicRule>(info, false, executionContext);
                OnLogicRuleExecuting(args);
                if (!args.Cancel){
                    ExecuteRule(info, executionContext);
                }
                OnLogicRuleExecuted(new LogicRuleExecutedEventArgs<TModelLogicRule>(info, executionContext));
            }
        }

        public virtual bool ContextIsValid(ExecutionContext executionContext, LogicRuleInfo<TModelLogicRule> logicRuleInfo) {
            return (logicRuleInfo.ExecutionContext | executionContext) == logicRuleInfo.ExecutionContext;
        }
    }
}