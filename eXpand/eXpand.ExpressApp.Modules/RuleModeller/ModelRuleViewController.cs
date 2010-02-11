using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;

namespace eXpand.ExpressApp.RuleModeller{
    public abstract class ModelRuleViewController<TModelRuleAttribute, TModelRuleNodeWrapper,
                                                      TModelRuleInfo, TModelRule> : ViewController
        where TModelRuleAttribute : ModelRuleAttribute where TModelRuleNodeWrapper : ModelRuleNodeWrapper
        where TModelRuleInfo : ModelRuleInfo<TModelRule>, new()
        where TModelRule : ModelRule
    {

        private bool isRefreshing;
        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas"+typeof(TModelRule).Name;
        
        public virtual bool IsReady{
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionReason executionReason){
            if (isReady){
                object currentObject = view.CurrentObject;
                foreach (TModelRule rule in ModelRuleManager<TModelRuleAttribute,TModelRuleNodeWrapper,TModelRuleInfo,TModelRule>.Instance[view.ObjectTypeInfo])
                    if (IsValidRule(rule, view))
                        ForceExecutionCore(currentObject, rule, invertCustomization, view, executionReason);
            }
            
        }

        public abstract void ExecuteRule(TModelRuleInfo info, ExecutionReason executionReason);


        /// <summary>
        /// An event that can be used to be notified whenever artifact begin customizing.
        /// </summary>
        public event EventHandler<ModelRuleExecutingEventArgs<TModelRuleInfo, TModelRule>> ModelRuleExecuting;

        /// <summary>
        /// An event that can be used to be notified whenever artifact state has been customized.
        /// </summary>
        public event EventHandler<ModelRuleExecutedEventArgs<TModelRuleInfo, TModelRule>> ModelRuleExecuted;
        

        private void FrameOnViewChanging(object sender, EventArgs args){
            if (View != null) InvertExecution(View);
            var view = ((ViewChangingEventArgs)args).View;
            Active[ActiveObjectTypeHasRules] = ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule>.HasRules(view);
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view, false, ExecutionReason.ViewChanging);
            var supportViewControlAdding = Frame.Template as ISupportViewControlAdding;
            if (supportViewControlAdding != null) 
                supportViewControlAdding.ViewControlAdding += (o, eventArgs) => ForceExecution(ExecutionReason.ViewControlAdding);
        }

        private void InvertExecution(View view){
            ForceExecution(Active[ActiveObjectTypeHasRules] && view != null && view.ObjectTypeInfo != null, view,true, ExecutionReason.ViewChanging);
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.ViewChanging+=FrameOnViewChanging;
        }

        protected override void OnActivated(){
            base.OnActivated();
            if (IsReady){
                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                ForceExecution(ExecutionReason.ViewActivated);
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            ForceExecution(ExecutionReason.ViewControlsCreated);
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
            ForceExecution(ExecutionReason.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e){
            isRefreshing = false;
            ForceExecution(ExecutionReason.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e){
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            if (!isRefreshing){
                ForceExecution(ExecutionReason.CurrentObjectChanged);
            }
        }

        private void ForceExecution(ExecutionReason executionReason,View view) {
            ForceExecution(IsReady, view, false, executionReason);
        }

        private void ForceExecution(ExecutionReason executionReason){
            ForceExecution(executionReason,View);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args){
            if (!String.IsNullOrEmpty(args.PropertyName)){
                ForceExecution(ExecutionReason.ObjectChanged);
            }
        }

        protected virtual void OnModelRuleExecuting(ModelRuleExecutingEventArgs<TModelRuleInfo,TModelRule> args){
            if (ModelRuleExecuting != null){
                ModelRuleExecuting(this, args);
            }
        }

        protected virtual void OnModelRuleExecuted(ModelRuleExecutedEventArgs<TModelRuleInfo,TModelRule> args){
            if (ModelRuleExecuted != null){
                ModelRuleExecuted(this, args);
            }
        }

        protected virtual bool IsValidRule(TModelRule rule, View view){
            return view != null &&(string.IsNullOrEmpty(rule.ViewId)||view.Id==rule.ViewId)&& view.ObjectTypeInfo != null &&
                   IsValidViewType(view, rule) && IsValidNestedType(rule,view)&&(rule.TypeInfo.IsAssignableFrom(view.ObjectTypeInfo));
        }

        private bool IsValidNestedType(TModelRule rule, View view){
            if (view is DetailView)
                return true;
            return (rule.Nesting == Nesting.Any || view.IsRoot);
        }

        private bool IsValidViewType(View view, TModelRule rule){
            return (rule.ViewType == ViewType.Any || (view is DetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType==ViewType.ListView));
        }


        protected virtual void ForceExecutionCore(object currentObject, TModelRule rule, bool invertCustomization, View view, ExecutionReason executionReason){
            TModelRuleInfo info = ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule>.CalculateModelRuleInfo(currentObject, rule);
            if (info != null){
                info.InvertingCustomization = invertCustomization;
                info.View = view;
                if (invertCustomization)
                    info.Active = !info.Active;
                var args = new ModelRuleExecutingEventArgs<TModelRuleInfo,TModelRule>(info, false,executionReason);
                OnModelRuleExecuting(args);
                if (!args.Cancel){
                    ExecuteRule(info, executionReason);
                }
                OnModelRuleExecuted(new ModelRuleExecutedEventArgs<TModelRuleInfo,TModelRule>(info,executionReason));
            }
        }

        
    }
}