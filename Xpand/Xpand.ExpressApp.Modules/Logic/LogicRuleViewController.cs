using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    public abstract class LogicRuleViewController<TModelLogicRule, TModule> : ViewController
        where TModelLogicRule : ILogicRule
        where TModule : XpandModuleBase, ILogicModuleBase {

        private bool isRefreshing;
        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas" + typeof(TModelLogicRule).Name;
        object _previousObject;
        XafApplication _application;


        public virtual bool IsReady {
            get { return Active.ResultValue && View != null && View.ObjectTypeInfo != null; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext, object currentObject,
                                           ActionBase action) {
            if (isReady && view != null) {
                var modelLogicRules = GetValidModelLogicRules(view);
                var logicRuleInfos = GetContextValidLogicRuleInfos(view, modelLogicRules, currentObject, executionContext, invertCustomization, action);
                foreach (var logicRuleInfo in logicRuleInfos) {
                    ForceExecutionCore(logicRuleInfo, executionContext);
                }
            }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext, object currentObject) {
            ForceExecution(isReady, view, invertCustomization, executionContext, currentObject, null);
        }

        protected IEnumerable<TModelLogicRule> GetValidModelLogicRules(View view) {
            return LogicRuleManager<TModelLogicRule>.Instance[view.ObjectTypeInfo].Where(rule => IsValidRule(rule, view)).OrderBy(rule => rule.Index);
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext) {
            ForceExecution(isReady, view, invertCustomization, executionContext, view == null ? null : view.CurrentObject);
        }

        protected virtual IEnumerable<LogicRuleInfo<TModelLogicRule>> GetContextValidLogicRuleInfos(View view, IEnumerable<TModelLogicRule> modelLogicRules, object currentObject, ExecutionContext executionContext, bool invertCustomization, ActionBase action) {
            return modelLogicRules.Select(rule => GetInfo(view, currentObject, rule, executionContext, invertCustomization, action)).Where(info => info != null);
        }

        void ForceExecutionCore(LogicRuleInfo<TModelLogicRule> logicRuleInfo, ExecutionContext executionContext) {
            var args = new LogicRuleExecutingEventArgs<TModelLogicRule>(logicRuleInfo, false, executionContext);
            OnLogicRuleExecuting(args);
            if (!args.Cancel) {
                ExecuteRule(logicRuleInfo, executionContext);
            }
            OnLogicRuleExecuted(new LogicRuleExecutedEventArgs<TModelLogicRule>(logicRuleInfo, executionContext));
        }

        LogicRuleInfo<TModelLogicRule> GetInfo(View view, object currentObject, TModelLogicRule rule, ExecutionContext executionContext, bool invertCustomization, ActionBase action) {
            if (action != null || ExecutionContextIsValid(executionContext, rule)) {
                LogicRuleInfo<TModelLogicRule> info = CalculateLogicRuleInfo(currentObject, rule, action);
                if (info != null && TemplateContextIsValid(info) && ViewIsRoot(info)) {
                    info.InvertingCustomization = invertCustomization;
                    if (info.InvertingCustomization) {
                        info.Active = !info.Active;
                    }
                    info.View = view;
                    return info;
                }
            }
            return null;
        }

        protected virtual bool ViewIsRoot(LogicRuleInfo<TModelLogicRule> info) {
            return !(info.Rule.IsRootView.HasValue) || info.Rule.IsRootView == View.IsRoot;
        }

        protected virtual bool TemplateContextIsValid(LogicRuleInfo<TModelLogicRule> info) {
            var frameTemplateContext = info.Rule.FrameTemplateContext;
            return frameTemplateContext != FrameTemplateContext.All ? frameTemplateContext + "Context" == Frame.Context : TemplateContextGroupIsValid(info);
        }

        bool TemplateContextGroupIsValid(LogicRuleInfo<TModelLogicRule> info) {
            var frameTemplateContextsGroup = GetModelLogic().FrameTemplateContextsGroup;
            var modelFrameTemplateContextsGroup = frameTemplateContextsGroup.FirstOrDefault(templateContexts => templateContexts.Id == info.Rule.FrameTemplateContextGroup);
            return modelFrameTemplateContextsGroup == null || modelFrameTemplateContextsGroup.FirstOrDefault(context => context.Name + "Context" == Frame.Context) != null;
        }


        public abstract void ExecuteRule(LogicRuleInfo<TModelLogicRule> info, ExecutionContext executionContext);



        public event EventHandler<LogicRuleExecutingEventArgs<TModelLogicRule>> LogicRuleExecuting;


        public event EventHandler<LogicRuleExecutedEventArgs<TModelLogicRule>> LogicRuleExecuted;


        void InvertExecution(View view, ExecutionContext executionContext, object currentObject) {
            bool hasRules = LogicRuleManager<TModelLogicRule>.HasRules(view);
            ForceExecution(hasRules && view != null && view.ObjectTypeInfo != null, view, true, executionContext, currentObject);
        }

        protected void InvertExecution(View view, ExecutionContext executionContext) {
            InvertExecution(view, executionContext, View.CurrentObject);
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged += FrameOnTemplateChanged;
            if (_application == null) {
                _application = Application;
                _application.ViewShowing += ApplicationOnViewShowing;
                _application.ViewCreating += ApplicationOnViewCreating;
            }
        }

        void ApplicationOnViewCreating(object sender, ViewCreatingEventArgs viewCreatingEventArgs) {
            if (Application != null) {
                var modelObjectView = Application.Model.Views.Single(modelView => modelView.Id == viewCreatingEventArgs.ViewID).AsObjectView;
                if (modelObjectView != null) {
                    var typeInfo = modelObjectView.ModelClass.TypeInfo;
                    bool hasRules = LogicRuleManager<TModelLogicRule>.HasRules(typeInfo);
                    ForceExecution(hasRules, null, false, ExecutionContext.ViewCreating);
                }
            }
        }

        void ApplicationOnViewShowing(object sender, ViewShowingEventArgs viewShowingEventArgs) {
            if (Application != null)
                ForceExecutioning(viewShowingEventArgs.View, ExecutionContext.ViewShowing);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Frame.ViewChanging -= FrameOnViewChanging;
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                if (_application != null) {
                    _application.ViewShowing -= ApplicationOnViewShowing;
                    _application.ViewCreating -= ApplicationOnViewCreating;
                }
            }
            base.Dispose(disposing);
        }
        void FrameOnViewChanging(object sender, ViewChangingEventArgs args) {
            ForceExecutioning(args.View, ExecutionContext.ViewChanging);
        }

        void ForceExecutioning(View view, ExecutionContext executionContext) {
            if (View != null) InvertExecution(View, executionContext);
            bool hasRules = LogicRuleManager<TModelLogicRule>.HasRules(view);
            ForceExecution(hasRules && view != null && view.ObjectTypeInfo != null, view, false, executionContext);
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewChanged = (Frame.Template) as ISupportViewChanged;
            if (supportViewChanged != null)
                supportViewChanged.ViewChanged += (o, args) => {
                    Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(args.View);
                    ForceExecution(ExecutionContext.ViewChanged, args.View);
                };
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady) {
                var actions = GetActions();
                foreach (var action in actions) {
                    var simpleAction = action as SimpleAction;
                    if (simpleAction != null)
                        simpleAction.Execute += ActionOnExecuted;
                    else {
                        var choiceAction = action as SingleChoiceAction;
                        if (choiceAction != null)
                            choiceAction.Execute += ActionOnExecuted;
                        else {
                            var parametrizedAction = action as ParametrizedAction;
                            if (parametrizedAction != null)
                                parametrizedAction.Execute += ActionOnExecuted;
                        }
                    }
                }
                View.SelectionChanged += ViewOnSelectionChanged;
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                ForceExecution(ExecutionContext.ControllerActivated);
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
                View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
                if (View is ListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
            }
        }

        IEnumerable<ActionBase> GetActions() {
            var modelActionExecutionContextGroup = GetModelLogic().ActionExecutionContextGroup;
            var actionContexts =
                (modelActionExecutionContextGroup.SelectMany(@group => @group,
                                                             (@group, executionContext) => executionContext.Name)).ToList();

            var actions =
                Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(
                    @base => actionContexts.Contains(@base.Id));
            return actions;
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            ForceExecution(actionBaseEventArgs);
        }

        void ForceExecution(ActionBaseEventArgs args) {
            Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(args.ShowViewParameters.CreatedView.ObjectTypeInfo);
            ForceExecution(IsReady, args.ShowViewParameters.CreatedView, false, ExecutionContext.None, args.ShowViewParameters.CreatedView.CurrentObject, args.Action);
        }


        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.ViewOnSelectionChanged);
        }


        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            ForceExecution(ExecutionContext.CustomProcessSelectedItem);
        }


        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.ObjectSpaceCommited);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ForceExecution(ExecutionContext.ViewControlsCreated);
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            var actions = GetActions();
            foreach (var action in actions) {
                var simpleAction = action as SimpleAction;
                if (simpleAction != null)
                    simpleAction.Execute -= ActionOnExecuted;
                else {
                    var choiceAction = action as SingleChoiceAction;
                    if (choiceAction != null)
                        choiceAction.Execute -= ActionOnExecuted;
                    else {
                        var parametrizedAction = action as ParametrizedAction;
                        if (parametrizedAction != null)
                            parametrizedAction.Execute -= ActionOnExecuted;
                    }
                }
            }
            View.SelectionChanged -= ViewOnSelectionChanged;
            if (ObjectSpace != null) {
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
                View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            }
            Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;

            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
            View.QueryCanChangeCurrentObject -= ViewOnQueryCanChangeCurrentObject;

            if (View is ListView)
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
        }

        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            _previousObject = View.CurrentObject;
        }


        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isRefreshing = false;
            if (View != null)
                ForceExecution(ExecutionContext.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e) {
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args) {
            if (_previousObject != null && !(ObjectSpace.IsDisposedObject(_previousObject))) {
                InvertExecution(View, ExecutionContext.CurrentObjectChanged, _previousObject);
                var notifyPropertyChanged = _previousObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
            if (!isRefreshing) {
                ForceExecution(ExecutionContext.CurrentObjectChanged);
                var notifyPropertyChanged = View.CurrentObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            ForceExecution(IsReady, View, false, ExecutionContext.NotifyPropertyObjectChanged, sender);
        }


        private void ForceExecution(ExecutionContext executionContext, View view) {
            ForceExecution(IsReady, view, false, executionContext);
        }

        private void ForceExecution(ExecutionContext executionContext) {
            ForceExecution(executionContext, View);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args) {
            if (!String.IsNullOrEmpty(args.PropertyName) && View != null)
                ForceExecution(ExecutionContext.ObjectSpaceObjectChanged);
        }

        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs<TModelLogicRule> args) {
            if (LogicRuleExecuting != null) {
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecutedEventArgs<TModelLogicRule> args) {
            if (LogicRuleExecuted != null) {
                LogicRuleExecuted(this, args);
            }
        }

        protected class ViewInfo {
            public ViewInfo(string viewId, bool isDetailView, bool isRoot, ITypeInfo objectTypeInfo, ViewEditMode? viewEditMode) {
                ViewId = viewId;
                IsDetailView = isDetailView;
                IsRoot = isRoot;
                ObjectTypeInfo = objectTypeInfo;
                ViewEditMode = viewEditMode;
            }

            public string ViewId { get; set; }
            public bool IsDetailView { get; set; }
            public bool IsRoot { get; set; }
            public ITypeInfo ObjectTypeInfo { get; set; }
            public ViewEditMode? ViewEditMode { get; set; }
        }
        protected virtual bool IsValidRule(TModelLogicRule rule, ViewInfo viewInfo) {
            return (IsValidViewId(viewInfo.ViewId, rule)) &&
                   IsValidViewType(viewInfo, rule) && IsValidNestedType(rule, viewInfo) && IsValidTypeInfo(viewInfo, rule) && IsValidViewEditMode(viewInfo, rule);
        }

        protected virtual bool IsValidViewEditMode(ViewInfo viewInfo, TModelLogicRule rule) {
            return !rule.ViewEditMode.HasValue || viewInfo.ViewEditMode == rule.ViewEditMode;
        }

        protected virtual bool IsValidRule(TModelLogicRule rule, View view) {
            var viewEditMode = view is DetailView ? ((DetailView)view).ViewEditMode : (ViewEditMode?)null;
            return view != null && IsValidRule(rule, new ViewInfo(view.Id, view is DetailView, view.IsRoot, view.ObjectTypeInfo, viewEditMode));
        }

        bool IsValidViewId(string viewId, TModelLogicRule rule) {
            if (rule.View != null)
                return viewId == rule.View.Id;
            var viewContextsGroup = GetModelLogic().ViewContextsGroup;
            var modelViewContexts = viewContextsGroup.FirstOrDefault(contexts => contexts.Id == rule.ViewContextGroup);
            return modelViewContexts == null ||
                   modelViewContexts.FirstOrDefault(context => context.Name == viewId) != null;
        }

        bool IsValidTypeInfo(ViewInfo viewInfo, TModelLogicRule rule) {
            return (((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(viewInfo.ObjectTypeInfo)) || IsValidDCTypeInfo(viewInfo, rule)) || rule.TypeInfo == null);
        }

        bool IsValidDCTypeInfo(ViewInfo viewInfo, TModelLogicRule rule) {
            if (viewInfo.ObjectTypeInfo.IsDomainComponent) {
                var entityType = XpoTypesInfoHelper.GetXpoTypeInfoSource().GetGeneratedEntityType(viewInfo.ObjectTypeInfo.Type);
                var types = new List<Type> { entityType };
                while (entityType != null && entityType != typeof(object)) {
                    entityType = entityType.BaseType;
                    types.Add(entityType);
                }
                return types.Contains(rule.TypeInfo.Type);
            }
            return true;
        }

        private bool IsValidNestedType(TModelLogicRule rule, ViewInfo viewInfo) {
            return viewInfo.IsDetailView || (rule.Nesting == Nesting.Any || viewInfo.IsRoot);
        }

        private bool IsValidViewType(ViewInfo viewInfo, TModelLogicRule rule) {
            return (rule.ViewType == ViewType.Any || (viewInfo.IsDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType == ViewType.ListView));
        }

        protected virtual LogicRuleInfo<TModelLogicRule> CalculateLogicRuleInfo(object targetObject, TModelLogicRule modelLogicRule, ActionBase action) {
            var logicRuleInfo = (LogicRuleInfo<TModelLogicRule>)Activator.CreateInstance(typeof(LogicRuleInfo<TModelLogicRule>));
            logicRuleInfo.Active = true;
            logicRuleInfo.Object = targetObject;
            logicRuleInfo.Rule = modelLogicRule;
            if (action == null)
                logicRuleInfo.ExecutionContext = CalculateCurrentExecutionContext(modelLogicRule.ExecutionContextGroup);
            else {
                logicRuleInfo.Action = action;
            }
            return logicRuleInfo;
        }

        ExecutionContext CalculateCurrentExecutionContext(string executionContextGroup) {
            IModelLogic modelLogic = GetModelLogic();
            var modelExecutionContexts = modelLogic.ExecutionContextsGroup.Single(context => context.Id == executionContextGroup);
            return modelExecutionContexts.Aggregate(ExecutionContext.None, (current, modelGroupContext) =>
                                                current | (ExecutionContext)Enum.Parse(typeof(ExecutionContext), modelGroupContext.Name.ToString(CultureInfo.InvariantCulture)));
        }

        public IModelLogic GetModelLogic() {
            return Application.Modules.FindModule<TModule>().GetModelLogic(Application.Model);
        }

        public virtual bool ExecutionContextIsValid(ExecutionContext executionContext, TModelLogicRule logicRuleInfo) {
            var context = CalculateCurrentExecutionContext(logicRuleInfo.ExecutionContextGroup);
            return (context | executionContext) == context;
        }
    }

}