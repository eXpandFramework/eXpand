using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleEvaluator  {
        static readonly IList<IModelLogic> _modelLogics=new List<IModelLogic>();

        public class ViewInfo {
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

        public Frame Frame { get; set; }

        protected virtual bool ViewIsRoot(LogicRuleInfo info) {
            return !(info.Rule.IsRootView.HasValue) || info.Rule.IsRootView == Frame.View.IsRoot;
        }

        public virtual bool ExecutionContextIsValid(ExecutionContext executionContext, ILogicRule logicRuleInfo) {
            var context = CalculateCurrentExecutionContext(logicRuleInfo);
            return (context | executionContext) == context;
        }

        protected virtual bool TemplateContextIsValid(LogicRuleInfo info) {
            var frameTemplateContext = info.Rule.FrameTemplateContext;
            return frameTemplateContext != FrameTemplateContext.All 
                ? frameTemplateContext + "Context" == Frame.Context : TemplateContextGroupIsValid(info);
        }

        LogicRuleInfo GetInfo(View view, object currentObject, ILogicRule rule, ExecutionContext executionContext, bool invertCustomization, ActionBase action) {
            if (action != null || ExecutionContextIsValid(executionContext, rule)) {
                var info = CalculateLogicRuleInfo(currentObject, rule, action);
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

        protected IEnumerable<ILogicRule> GetValidModelLogicRules(View view) {
            return LogicRuleManager.Instance[view.ObjectTypeInfo].Where(rule => IsValidRule(rule, view)).OrderBy(rule => rule.Index);
        }

        public virtual IEnumerable<LogicRuleInfo> GetContextValidLogicRuleInfos(View view,  object currentObject, ExecutionContext executionContext, bool invertCustomization, ActionBase action) {
            var modelLogicRules = GetValidModelLogicRules(view);
            var logicRuleInfos = modelLogicRules.Select(rule => GetInfo(view, currentObject, rule, executionContext, invertCustomization, action));
            return logicRuleInfos.Where(info => info != null);
        }

        public virtual bool IsValidRule(ILogicRule rule, ViewInfo viewInfo) {
            return (IsValidViewId(viewInfo.ViewId, rule)) &&
                   IsValidViewType(viewInfo, rule) && IsValidNestedType(rule, viewInfo) && IsValidTypeInfo(viewInfo, rule) 
                   && IsValidViewEditMode(viewInfo, rule);
        }

        protected virtual bool IsValidViewEditMode(ViewInfo viewInfo, ILogicRule rule) {
            return !rule.ViewEditMode.HasValue || viewInfo.ViewEditMode == rule.ViewEditMode;
        }

        bool TemplateContextGroupIsValid(LogicRuleInfo info) {
            var frameTemplateContextsGroup = GetModelLogic(info.Rule).FrameTemplateContextsGroup;
            var modelFrameTemplateContextsGroup = frameTemplateContextsGroup.FirstOrDefault(templateContexts 
                => templateContexts.Id == info.Rule.FrameTemplateContextGroup);
            return modelFrameTemplateContextsGroup == null || modelFrameTemplateContextsGroup.FirstOrDefault(context 
                => context.Name + "Context" == Frame.Context) != null;
        }

        protected virtual bool IsValidRule(ILogicRule rule, View view) {
            var viewEditMode = view is DetailView ? ((DetailView)view).ViewEditMode : (ViewEditMode?)null;
            return view != null && IsValidRule(rule, new ViewInfo(view.Id, view is DetailView, view.IsRoot, view.ObjectTypeInfo, viewEditMode));
        }

        bool IsValidViewId(string viewId, ILogicRule rule) {
            if (rule.View != null)
                return viewId == rule.View.Id;
            var viewContextsGroup = GetModelLogic(rule).ViewContextsGroup;
            var modelViewContexts = viewContextsGroup.FirstOrDefault(contexts => contexts.Id == rule.ViewContextGroup);
            return modelViewContexts == null ||modelViewContexts.FirstOrDefault(context => context.Name == viewId) != null;
        }

        bool IsValidTypeInfo(ViewInfo viewInfo, ILogicRule rule) {
            return (((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(viewInfo.ObjectTypeInfo)) 
                || IsValidDCTypeInfo(viewInfo, rule)) || rule.TypeInfo == null);
        }

        bool IsValidDCTypeInfo(ViewInfo viewInfo, ILogicRule rule) {
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

        private bool IsValidNestedType(ILogicRule rule, ViewInfo viewInfo) {
            return viewInfo.IsDetailView || (rule.Nesting == Nesting.Any || viewInfo.IsRoot);
        }

        private bool IsValidViewType(ViewInfo viewInfo, ILogicRule rule) {
            return (rule.ViewType == ViewType.Any || (viewInfo.IsDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType == ViewType.ListView));
        }

        protected virtual LogicRuleInfo CalculateLogicRuleInfo(object targetObject, ILogicRule modelLogicRule, ActionBase action) {
            var logicRuleInfo = new LogicRuleInfo{Active = true, Object = targetObject, Rule = modelLogicRule};
            if (action == null)
                logicRuleInfo.ExecutionContext = CalculateCurrentExecutionContext(modelLogicRule);
            else {
                logicRuleInfo.Action = action;
            }
            CalculateLogicRuleInfo(logicRuleInfo);
            return logicRuleInfo;
        }

        public bool Fit(object targetObject, ILogicRule logicRule) {
            var criteria = CriteriaOperator.Parse(logicRule.NormalCriteria);
            return targetObject != null? criteria.Fit(targetObject): string.IsNullOrEmpty(logicRule.EmptyCriteria) 
                || CriteriaOperator.Parse(logicRule.EmptyCriteria).Fit(new object());
        }

        void CalculateLogicRuleInfo(LogicRuleInfo calculateLogicRuleInfo) {
            calculateLogicRuleInfo.Active = Fit(calculateLogicRuleInfo.Object, calculateLogicRuleInfo.Rule);
        }

        ExecutionContext CalculateCurrentExecutionContext(ILogicRule logicRule) {
            var modelLogic = GetModelLogic(logicRule);
            var modelExecutionContexts = modelLogic.ExecutionContextsGroup.Single(context => context.Id == logicRule.ExecutionContextGroup);
            return modelExecutionContexts.Aggregate(ExecutionContext.None, (current, modelGroupContext) =>current | 
                (ExecutionContext)Enum.Parse(typeof(ExecutionContext), modelGroupContext.Name.ToString(CultureInfo.InvariantCulture)));
        }

        IModelLogic GetModelLogic(ILogicRule logicRule) {
            var typesInfo = Frame.Application.TypesInfo;
            return _modelLogics.Select(logic 
                => new { logic, Info = typesInfo.FindTypeInfo(logic.GetType()) })
                .Select(arg => new{arg.logic,Attribute = arg.Info.ImplementedInterfaces.Select(info 
                    => info.FindAttribute<ModelLogicValidRuleAttribute>()).Single(attribute => attribute!=null)})
                .Where(arg => arg.Attribute.RuleType.IsInstanceOfType(logicRule))
                .Select(arg => arg.logic).Single();
        }

        internal static IList<IModelLogic> ModelLogics {
            get { return _modelLogics; }
        }
    }
}